using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumers
{
    //NOTE: MassTransit is convention based and expects our events (the one in Contracts) to end with Consumer
    public class ActionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;
        public ActionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
            
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("-->Consuming action created: " + context.Message.Id);
            var item = _mapper.Map<Item>(context.Message);
            await item.SaveAsync();
        }
    }
}