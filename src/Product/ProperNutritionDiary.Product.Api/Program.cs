using ProperNutritionDiary.Product.Application;
using ProperNutritionDiary.Product.Persistence;
using ProperNutritionDiary.Product.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("mysql")!,
    builder.Configuration["Cassandra:Host"]!,
    builder.Configuration["Cassandra:KeySpace"]!,
    builder.Configuration["Cassandra:Name"]!,
    builder.Configuration["Cassandra:Password"]!
);

builder.Services.AddApplication(
    typeof(ProperNutritionDiary.Product.Application.DependencyInjection).Assembly,
    typeof(ProperNutritionDiary.Product.Domain.User.User).Assembly
);

builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();

app.AddPresentation();

app.Run();
