using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
        //options.TokenValidationParameters.ValidIssuers = new string [] {"http://localhost:5000", "http://identity-svc"};
    });

//Add Cors policy. Needed for browser to SingleR server connection
builder.Services.AddCors(options => {
    options.AddPolicy("customPolicy", b => {
        b.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
        .WithOrigins(builder.Configuration["ClientApp"]);
    });
});
var app = builder.Build();
app.UseCors();
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
