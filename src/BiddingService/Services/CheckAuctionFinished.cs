using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services
{
    //Derived from base class for implementing a long running service: BackgroundService
    //It runs as singleton. Runs when application starts up and stops when application shuts down
    //It must implement ExecuteAsync method
    public class CheckAuctionFinished : BackgroundService
    {
        private readonly ILogger<CheckAuctionFinished> _logger;
        private readonly IServiceProvider _services;

        public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Check for finished auctions");
            
            //Register a deligate when cancelation is called
            stoppingToken.Register(() => _logger.LogInformation("==> Auction check is stopping"));

            while(!stoppingToken.IsCancellationRequested)
            {
                await CheckAuctions(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckAuctions(CancellationToken stoppingToken)
        {
            var finishedAuctions = await DB.Find<Auction>()
            .Match(x=>x.AuctionEnd <= DateTime.UtcNow )
            .Match(x=> !x.Finished)
            .ExecuteAsync(stoppingToken);

            //Create a scope to access IPublishEndpoint, because it is different from Background service 
            //masstransit scope is scoped to the request
            if(finishedAuctions.Count == 0) return;
            _logger.LogInformation($"==> Found {finishedAuctions.Count} auctions that have completed ");
            using var scope =_services.CreateScope();
            var endPoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach (var auction in finishedAuctions)
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                var winningBid = await DB.Find<Bid>()
                    .Match(a=>a.AuctionId == auction.ID)
                    .Match(b=>b.BidStatus == BidStatus.Accepted)
                    .Sort(x=>x.Descending(s=>s.Amount))
                    .ExecuteFirstAsync(stoppingToken);
                
                await endPoint.Publish(new AuctionFinished
                    {
                        ItemSold = winningBid != null,
                        AuctionId = auction.ID,
                        Winner = winningBid?.Bidder,
                        Amount = winningBid?.Amount,
                        Seller = auction.Seller
                    }, stoppingToken); 
            }

        }
    }
}