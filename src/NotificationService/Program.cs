using MassTransit;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);


//Configure MassTransit. Package: MassTransit.RabbitMQ
builder.Services.AddMassTransit(x=>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    //add dashes in formater as "a-b-c" for consumer names
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, cfg)=>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    } );
});
builder.Services.AddSignalR();
var app = builder.Build();

//map root of notification hub
app.MapHub<NotificationHub>("/notifications");
app.Run();
