using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Carter;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Logging;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProperNutritionDiary.DiaryApi.Db;
using ProperNutritionDiary.DiaryApi.Diary;
using ProperNutritionDiary.DiaryApi.Diary.Create;
using ProperNutritionDiary.DiaryApi.Product;
using Refit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder
    .Configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables("PROPER_NUTRITION_DIARY");

builder.Services.AddDbContext<AppCtx>(conf =>
{
    Console.WriteLine(builder.Configuration.GetConnectionString("mssql"));

    conf.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
});

builder.Services.AddCarter(new DependencyContextAssemblyCatalog([typeof(DiaryModule).Assembly]));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        }
    );

    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        }
    );

    opt.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            // Здесь указаны стандартные настройки для подключения к RabbitMq
            cfg.Host(
                builder.Configuration["RabbitMQPath"]
                    ?? throw new ConfigurationException("RabbitMQ path is not set"),
                "/",
                h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                }
            );
            cfg.ConfigureEndpoints(context);
        }
    );
});

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"] ?? throw new InvalidConfigurationException()
                )
            ),
            ClockSkew = TimeSpan.FromSeconds(1),
        };

        options.IncludeErrorDetails = true;
    });

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("guest", c => c.RequireRole("guest", "plain"))
    .AddPolicy("plain", c => c.RequireRole("plain"));

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

builder
    .Services.AddRefitClient<IProductApi>()
    .ConfigureHttpClient(cc =>
    {
        cc.BaseAddress = new Uri(new Uri(builder.Configuration["GatewayPath"]!), "product-api");
        cc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            builder.Configuration["ServerToken"]!
        );

        Console.WriteLine(new Uri(new Uri(builder.Configuration["GatewayPath"]!), "product-api"));
        Console.WriteLine(
            new AuthenticationHeaderValue("Bearer", builder.Configuration["ServerToken"]!)
        );
    })
    .ConfigurePrimaryHttpMessageHandler(
        () =>
            new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            }
    );

builder.Services.AddTransient<IDiaryService, DiaryService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

app.MapGet("/", () => "Hello World!");

app.UseSwagger();
app.UseSwaggerUI();

await app.RunAsync();
