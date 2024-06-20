using System.Text;
using Carter;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProperNutritionDiary.UserStatsApi.Db;
using ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;
using Serilog;

const string MainCorsPolicy = "mainCors";

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder
    .Configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables("PROPER_NUTRITION_DIARY");

builder.Services.AddDbContext<AppCtx>(conf =>
{
    conf.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
});

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddTransient<DailyStatsService>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    var assembly = typeof(Program).Assembly;
    x.AddConsumers(assembly);
    x.UsingRabbitMq(
        (context, cfg) =>
        {
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MainCorsPolicy,
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true);
        }
    );
});

builder.Services.AddCarter(
    new DependencyContextAssemblyCatalog([typeof(UserTotalDailiesStatsModule).Assembly])
);

var app = builder.Build();

app.UseCors(MainCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapCarter();

app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
