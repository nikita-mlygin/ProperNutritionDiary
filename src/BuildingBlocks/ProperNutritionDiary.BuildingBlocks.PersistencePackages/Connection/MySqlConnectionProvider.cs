using System.Data;
using MySqlConnector;

namespace ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;

public class MySqlConnectionProvider(string connectionString) : IConnectionProvider
{
    private readonly string connectionString = connectionString;

    public Task<IDbConnection> GetConnection()
    {
        var connection = new MySqlConnection(connectionString);

        return Task.FromResult(connection as IDbConnection);
    }
}
