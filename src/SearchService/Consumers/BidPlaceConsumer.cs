﻿using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    public class BidPlaceConsumer : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("Search --> Consumming bid placed");
            var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);
            if( context.Message.BidStatus.Contains("Accepted") 
                && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await auction.SaveAsync();
            }
        }
    }
}
