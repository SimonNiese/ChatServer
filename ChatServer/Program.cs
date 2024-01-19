using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using ChatServer.Model;
using FluentArgs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServiceStack.Redis;
using StackExchange.Redis;

public class Program{
    public static Config Config { get; set; }
    public static void Main(string[] args) {
        

        FluentArgsBuilder.New()
            .Parameter<string>("-c", "--cache")
            .IsRequired()
            .Parameter<int>("-p", "--port")
            .IsRequired()
            .Call(port => cache => {
                Config = new Config() {
                    CacheUrl = cache,
                    Port = port
                };
                Console.WriteLine("Config:\n"+
                                  $"\tCache-address: {Config.CacheUrl}"+
                                  $"\tPort: {Config.Port}");
            })
            .Parse(args);

        
        var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
        builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Any, 8080));
        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(
                policy => {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
        

// Redis integration
        builder.Services.AddSingleton<RedisClient>(new RedisClient(new RedisEndpoint(Config.CacheUrl, Config.Port)));

        
        var app = builder.Build();
        
// Configure the HTTP request pipeline.
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();

        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}