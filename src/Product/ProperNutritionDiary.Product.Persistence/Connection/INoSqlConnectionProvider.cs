using Cassandra.Mapping;

namespace ProperNutritionDiary.Product.Persistence.Connection;

public interface INoSqlConnectionProvider
{
    public Task<IMapper> Get();
}
