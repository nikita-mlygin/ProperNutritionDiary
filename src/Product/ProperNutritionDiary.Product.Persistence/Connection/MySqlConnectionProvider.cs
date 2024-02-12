namespace ProperNutritionDiary.Product.Persistence.Connection;

public class MySqlConnectionProvider(string connectionString)
    : BuildingBlocks.PersistencePackages.Connection.MySqlConnectionProvider(connectionString),
        ISqlConnectionProvider { }
