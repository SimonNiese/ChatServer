using System.Reflection.Metadata;
using System.Text.Json;
using ChatServer.Model;
using FluentArgs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;

public class Program{
    public static Config Config { get; set; }
    public static void Main(string[] args) {
        

        FluentArgsBuilder.New()
            .Parameter<string>("-c", "--cache")
            .IsRequired()
            .Parameter<string>("-k", "--keycloak")
            .IsRequired()
            .Parameter<string>("-r", "--realm")
            .IsRequired()
            .Call(realm => keycloak => cache => {
                Config = new Config() {
                    CacheUrl = cache,
                    KeycloakUrl = keycloak,
                    Realm = realm
                };
            })
            .Parse(args);


        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// Keycloak integration
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = Config.KeycloakUrl;
                options.Audience = "my_client";
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = Config.KeycloakUrl + "/" + Config.Realm + "/.well-known/openid-configuration";

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        // Customize the error response
                        var message = "Authentication failed: " + context.Exception.Message;
                        var errorResponse = JsonSerializer.Serialize(new { message });
                        return context.Response.WriteAsync(errorResponse);
                    }
                };
            });

// Redis integration
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Config.CacheUrl));


        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}