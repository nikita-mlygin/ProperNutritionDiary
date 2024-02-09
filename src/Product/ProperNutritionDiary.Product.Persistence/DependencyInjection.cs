using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages;

namespace ProperNutritionDiary.Product.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddPersistenceBuildingBlocks();

        return services;
    }
}
