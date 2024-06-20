using DiaryMessageStore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MessageStore>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    // Добавляем консьюмеров
    var assembly = typeof(Program).Assembly;
    x.AddConsumers(assembly);
    x.UsingRabbitMq(
        (context, cfg) =>
        {
            //Здесь указаны стандартные настройки для подключения к RabbitMq
            cfg.Host(
                "localhost",
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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/messages",
    (MessageStore messageStore) =>
    {
        return Results.Ok(messageStore.GetAllMessages());
    }
);

app.Run();
