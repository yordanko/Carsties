

using Contracts;
using MassTransit;
using MongoDB.Entities;
using Polly;

namespace SearchService.Consumers;
public class ActionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("-->Consuming action deleted: " + context.Message.Id);
        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if(!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionUpdated), "Problem deleting mongodb");
    }
}