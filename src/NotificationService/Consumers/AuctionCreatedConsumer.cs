using System;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService
{
    //Server side of SingleR 
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        //Inject SingleR hub
        public AuctionCreatedConsumer(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
           Console.WriteLine("==> auction created message received");

            //send notification to all connected clients, 
            //When client listed for method AuctionCreated, when received then client received context.Message from server 
           await _hubContext.Clients.All.SendAsync("AuctionCreated", context.Message);
        }
    }
}
