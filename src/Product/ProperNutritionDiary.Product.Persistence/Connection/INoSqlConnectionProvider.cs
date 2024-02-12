using Cassandra.Mapping;
using DomainDesignLib.Persistence.Repository;

namespace ProperNutritionDiary.Product.Persistence.Connection;

public interface INoSqlConnectionProvider
{
    public Task<IMapper> Get();
}
