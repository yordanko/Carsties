using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services
{
    //This class is used as client for gRPC call
    public class GrpcAuctionClient
    {
        private readonly ILogger<GrpcAuctionClient> _logger;
        private readonly IConfiguration _config;

        public GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Auction GetAuction(string id)
        {
            _logger.LogInformation("Calling GRPC Service");

            var channel = GrpcChannel.ForAddress(_config["GrpcAuction"]);
            var client = new GrpcAuction.GrpcAuctionClient(channel);
            var request = new GetAuctionRequest { Id = id};

            try
            {
                var reply = client.GetAuction(request);
                var auction = new Auction
                {
                    ID = reply.Auction.Id,
                    AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                    Seller = reply.Auction.Seller,
                    ReservePrice = reply.Auction.ReservedPrice
                };

                return auction;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Could not call GRPC server");
                return null;
            }
        }
    }
}