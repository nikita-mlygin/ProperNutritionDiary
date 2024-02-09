using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Persistence.Product;

namespace ProperNutritionDiary.Product.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddPersistenceBuildingBlocks();

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
