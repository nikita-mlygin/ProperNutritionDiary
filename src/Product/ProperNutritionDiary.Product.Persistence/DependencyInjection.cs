using Cassandra.Mapping;
using DomainDesignLib.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Persistence.Connection;
using ProperNutritionDiary.Product.Persistence.Mappings;
using ProperNutritionDiary.Product.Persistence.Product;
using ProperNutritionDiary.Product.Persistence.Product.Summary;

namespace ProperNutritionDiary.Product.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string mysqlConnectionString,
        string cassandraHost,
        string cassandraKeySpace,
        string cassandraUserName,
        string cassandraPassword
    )
    {
        services.AddPersistenceBuildingBlocks();

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

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductSummaryRepository, ProductSummaryRepository>();

        MappingConfiguration.Global.Define<GlobalMappingsDefinition>();

        return services;
    }
}
