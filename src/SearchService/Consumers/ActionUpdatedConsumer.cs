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
    public class ActionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public ActionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("-->Consuming action updated: " + context.Message.Id);
            var item = _mapper.Map<Item>(context.Message);
            var result = await DB.Update<Item>()
            .MatchID(item.ID)
            .ModifyOnly(b=> new {b.Make, b.Model, b.Color, b.Mileage, b.Year}, item)
            .ExecuteAsync();
                     
            if(!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
        }
    }
}