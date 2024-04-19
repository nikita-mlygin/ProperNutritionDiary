using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.UserStatsApi.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppCtx>(conf =>
{
    conf.UseSqlServer(builder.Configuration.GetConnectionString("mssql"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
