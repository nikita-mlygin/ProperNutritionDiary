using System.Data;

namespace ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;

public interface IConnectionProvider
{
    Task<IDbConnection> GetConnection();
}
