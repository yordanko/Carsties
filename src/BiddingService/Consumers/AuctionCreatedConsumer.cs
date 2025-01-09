using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers
{
    //This class will be used by Masstransit so must inherite IConsumer interface
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            var auction = new Auction
            {
                ID = context.Message.Id.ToString(),
                Seller = context.Message.Seller,
                AuctionEnd = context.Message.AuctionEnd,
                ReservePrice = context.Message.ReservePrice,
                Finished = context.Message.AuctionEnd <= DateTime.UtcNow ? true : false
            };

            //Either line will work. Note: Mongo.Entities.DB is static class, no need to be injected
            //await DB.SaveAsync(auction);  
            await auction.SaveAsync();
            
        }
    }
}