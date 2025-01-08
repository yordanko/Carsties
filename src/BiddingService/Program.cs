using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BiddingService.Consumers;
using System.Reflection;
using BiddingService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Configure MassTransit. Package: MassTransit.RabbitMQ
builder.Services.AddMassTransit(x=>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    //add dashes in formater as "a-b-c" for consumer names
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

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

// Add Authentication (same as auction service) for package: Microsoft.AspNetCore.Authentication.JwtBearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";

        //This is list of valid tocken issuer. It is needed because we can run in docker or in debug/run local pc
        // options.TokenValidationParameters.ValidIssuers = new string [] {"http://localhost:5000", "http://identity-svc"};
        // options.TokenValidationParameters.ValidateIssuerSigningKey = false;
        //options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz123456"));
        
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<CheckAuctionFinished>();
builder.Services.AddScoped<GrpcAuctionClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

//Initialize mongo db connection settings. Package: MongoDB.Entities
await DB.InitAsync("BidDb", MongoClientSettings
.FromConnectionString(builder.Configuration.GetConnectionString("BidDbConnection")));

app.Run();
