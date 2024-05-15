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
            Console.WriteLine("--> Consumming bid placed");
            var auction = await _dbContext.Auctions.FindAsync(context.Message.Id);
            if(auction.CurrentHighBid==null 
                || context.Message.BidStatus.Contains("Accepted") 
                && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
            }


        }
    }
}
