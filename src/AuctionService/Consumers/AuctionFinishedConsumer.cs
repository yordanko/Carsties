﻿//using AuctionService.Entities;
using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    //NOTE: Thsi is part of data consistency.  
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _dbContext;

        public AuctionFinishedConsumer(AuctionDbContext context)
        {
            _dbContext = context;
        }
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consumming auction finished");
            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);
            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }
            auction.Status = auction.SoldAmount > auction.ReservePrice 
                ? Entities.Status.Finished : Entities.Status.ReserveNotMet;

            await _dbContext.SaveChangesAsync();
                
        }
    }
}
