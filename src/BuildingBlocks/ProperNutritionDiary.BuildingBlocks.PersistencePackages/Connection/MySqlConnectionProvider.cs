using System.Data.Common;
using DomainDesignLib.Persistence.Repository;
using MySqlConnector;

namespace ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;

public class MySqlConnectionProvider(string connectionString) : IConnectionProvider
{
    private readonly string connectionString = connectionString;

    public Task<DbConnection> Get()
    {
        var connection = new MySqlConnection(connectionString);

        return Task.FromResult(connection as DbConnection);
    }
}
