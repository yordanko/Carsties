using MassTransit;
using Contracts;
using AuctionService.Data;

namespace AuctionService.Consumers
{
    public class BidPlaceConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _dbContext;

        public BidPlaceConsumer(AuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Consume(ConsumeContext<BidPlaced> context)
        { 
            Console.WriteLine("Auction --> Consumming bid placed");
            var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
            if( auction.CurrentHighBid == null || (context.Message.BidStatus.Contains("Accepted") 
                && context.Message.Amount > auction.CurrentHighBid))
            {
                Console.WriteLine($"Auction --> Consumming bid placed --> Save ammount {context.Message.Amount}");
                auction.CurrentHighBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
            }


        }
    }
}
