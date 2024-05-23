using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//configure Poli retry policy
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

//Configure MassTransit
builder.Services.AddMassTransit(x=>
{
    x.AddConsumersFromNamespaceContaining<ActionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg)=>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        //configure retry attemps if databse is down
        //NOTE: This retry is configured only for create queue!
        cfg.ReceiveEndpoint("search-action-created", e =>
        {
            e.UseMessageRetry(r=>r.Interval(5,5));
            //specific consumer
            e.ConfigureConsumer<ActionCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    } );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

app.Run();

//Poly configuration
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        //.WaitAndRetryAsync(5, _=> TimeSpan.FromSeconds(3));
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));