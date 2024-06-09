using System;
using Cassandra.Mapping;
using DomainDesignLib.Persistence;
using DomainDesignLib.Persistence.Repository.Hooks;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Persistence.Connection;
using ProperNutritionDiary.Product.Persistence.ExternalProduct;
using ProperNutritionDiary.Product.Persistence.Mappings;
using ProperNutritionDiary.Product.Persistence.Product;
using ProperNutritionDiary.Product.Persistence.Product.OpenFoodFacts;
using ProperNutritionDiary.Product.Persistence.Product.Summary;
using ProperNutritionDiary.Product.Persistence.Product.Usda;
using Refit;

namespace ProperNutritionDiary.Product.Persistence;

public static class DependencyInjection
{
    private const string UsdaPath = "https://api.nal.usda.gov/fdc/v1";
    private const string OpenApiPath = "https://world.openfoodfacts.net/api/v3/";
    private const string OpenApiSearchPath = "https://world.openfoodfacts.org/";

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPersistenceBuildingBlocks();
        services.AddEntityTracking();
        services.AddEventDispatcher();

        var mysqlConnectionString = configuration.GetConnectionString("mysql")!;
        var cassandraHost = configuration["Cassandra:Host"]!;
        var cassandraKeySpace = configuration["Cassandra:KeySpace"]!;
        var cassandraUserName = configuration["Cassandra:Name"]!;
        var cassandraPassword = configuration["Cassandra:Password"]!;
        var usdaClientApiKey = configuration["Usda:ApiKey"]!;

        services.AddScoped<ISqlConnectionProvider, MySqlConnectionProvider>(
            _ => new MySqlConnectionProvider(mysqlConnectionString)
        );
        services.Decorate<ISqlConnectionProvider, MySqlConnectionProviderDecorator>();

        services.AddScoped<INoSqlConnectionProvider, NoSqlConnectionProvider>(
            _ => new NoSqlConnectionProvider(
                cassandraHost,
                cassandraKeySpace,
                cassandraUserName,
                cassandraPassword
            )
        );

        services
            .AddRefitClient<IUsdaApi>()
            .ConfigureHttpClient(cfg =>
            {
                cfg.DefaultRequestHeaders.Add("X-Api-Key", usdaClientApiKey);
                cfg.BaseAddress = new Uri(UsdaPath);
            })
            .AddHttpMessageHandler(serviceProvider => new HttpLoggingHandler(
                serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()
            ));

        services
            .AddRefitClient<IOpenFoodFactsApi>()
            .ConfigureHttpClient(cfg =>
            {
                cfg.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
                );
                cfg.BaseAddress = new Uri(OpenApiPath);
            })
            .AddHttpMessageHandler(serviceProvider => new HttpLoggingHandler(
                serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()
            ));

        services
            .AddRefitClient<IOpenFoodFactsSearchApi>()
            .ConfigureHttpClient(cfg =>
            {
                cfg.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
                );
                cfg.BaseAddress = new Uri(OpenApiSearchPath);
            })
            .AddHttpMessageHandler(serviceProvider => new HttpLoggingHandler(
                serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()
            ));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductSummaryRepository, ProductSummaryRepository>();

        services.AddSingleton<UsdaConverter>();

        services.AddScoped<IExternalProductRepository, ExternalProductRepository>();

        services.AddEasyCaching(options =>
        {
            var redisConfig = configuration.GetSection("ConnectionStrings:Redis").Value;

            options.WithJson();

            services.AddEasyCaching(option =>
            {
                option.WithJson("json"); // Register JSON serializer

                option.UseRedis(
                    config =>
                    {
                        var redisConfig = configuration.GetSection("ConnectionStrings:Redis").Value;
                        config.DBConfig.Endpoints.Add(
                            new EasyCaching.Core.Configurations.ServerEndPoint(
                                redisConfig.Split(':')[0],
                                int.Parse(redisConfig.Split(':')[1])
                            )
                        );
                        config.DBConfig.Database = 0;
                        // config.DBConfig.Password = configuration[
                        //     "EasyCaching:Redis:DBConfig:Password"
                        // ];
                        config.SerializerName = "json"; // Use the JSON serializer
                    },
                    "redis1"
                );
            });
        });

        services.Decorate<IExternalProductRepository, CachedExternalProductRepository>();

        MappingConfiguration.Global.Define<GlobalMappingsDefinition>();

        services.AddSingleton(_ => DbLoggingConfiguration.Default);

        return services;
    }
}
