using System.Runtime.CompilerServices;
using Cassandra.Mapping;
using DomainDesignLib.Persistence;
using DomainDesignLib.Persistence.Repository.Hooks;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Persistence.Connection;
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
        string mysqlConnectionString,
        string cassandraHost,
        string cassandraKeySpace,
        string cassandraUserName,
        string cassandraPassword,
        string usdaClientApiKey
    )
    {
        services.AddPersistenceBuildingBlocks();
        services.AddEntityTracking();
        services.AddEventDispatcher();

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
            });

        services
            .AddRefitClient<IOpenFoodFactsApi>()
            .ConfigureHttpClient(cfg =>
            {
                cfg.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
                );
                cfg.BaseAddress = new Uri(OpenApiPath);
            });

        services
            .AddRefitClient<IOpenFoodFactsSearchApi>()
            .ConfigureHttpClient(cfg =>
            {
                cfg.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
                );
                cfg.BaseAddress = new Uri(OpenApiSearchPath);
            });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductSummaryRepository, ProductSummaryRepository>();

        services.AddSingleton<UsdaConverter>();

        services.AddScoped<IExternalProductRepository, ExternalProductRepository>();

        MappingConfiguration.Global.Define<GlobalMappingsDefinition>();

        services.AddSingleton(_ => DbLoggingConfiguration.Default);

        return services;
    }
}
