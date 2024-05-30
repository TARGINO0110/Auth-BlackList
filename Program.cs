using Auth_BlackList.Model;
using Auth_BlackList.Security;
using Auth_BlackList.Services;
using Auth_BlackList.Services.Interfaces;
using Auth_BlackList.TokenAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddStackExchangeRedisCache(r =>
{
    r.InstanceName = builder.Configuration.GetSection("RedisConfig:InstanceName").Value;
    r.Configuration = builder.Configuration.GetSection("RedisConfig:Configuration").Value;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IRedisCachingService, RedisCachingService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ClientRequestIpFilterMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

RouteGroupBuilder authEndpoint = app.MapGroup("/api/v1/auth/");
authEndpoint.MapPost("token",
    [HttpPost][AllowAnonymous] async ([FromBody] Auth auth, IRedisCachingService redisService, IHttpContextAccessor httpContextAccessor) =>
    {
        string ipRequest = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        if (!auth.ValidUser)
        {
            await redisService.ValidBlackListIpAsync(ipRequest);
            return Results.Unauthorized();
        }

        await redisService.RemoveCachingAsync(ipRequest);

        return Results.Ok(new GenerateJWTToken().GenerateTokenJWT(auth));
    })
.WithName("AuthToken")
.WithOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();

app.Run();
