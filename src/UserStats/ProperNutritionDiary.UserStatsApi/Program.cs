using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.UserStatsApi.Db;

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
