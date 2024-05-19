using ProperNutritionDiary.Product.Application;
using ProperNutritionDiary.Product.Persistence;
using ProperNutritionDiary.Product.Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder
    .Configuration.SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables("PROPER_NUTRITION_DIARY");

builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("mysql")!,
    builder.Configuration["Cassandra:Host"]!,
    builder.Configuration["Cassandra:KeySpace"]!,
    builder.Configuration["Cassandra:Name"]!,
    builder.Configuration["Cassandra:Password"]!,
    builder.Configuration["FsdaKey"]!
);

builder.Services.AddApplication(
    typeof(ProperNutritionDiary.Product.Application.DependencyInjection).Assembly,
    typeof(ProperNutritionDiary.Product.Domain.User.User).Assembly
);

builder.Services.AddPresentation(builder.Configuration);

builder.Host.UseSerilog(
    (ctx, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration);
    }
);

var app = builder.Build();

app.AddPresentation();

app.Run();
