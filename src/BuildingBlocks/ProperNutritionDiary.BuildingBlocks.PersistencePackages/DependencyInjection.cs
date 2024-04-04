using Dapper;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages.Dapper;

namespace ProperNutritionDiary.BuildingBlocks.PersistencePackages;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceBuildingBlocks(
        this IServiceCollection serviceCollection
    )
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));

        return serviceCollection;
    }
}
