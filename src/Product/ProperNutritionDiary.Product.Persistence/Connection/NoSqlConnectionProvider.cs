using System.Data.Common;
using Cassandra;
using Cassandra.Data;
using Cassandra.Mapping;

namespace ProperNutritionDiary.Product.Persistence.Connection;

public class NoSqlConnectionProvider(string host, string keySpace, string userName, string password)
    : INoSqlConnectionProvider
{
    private readonly string keySpace = keySpace;

    private readonly Cluster cluster = Cluster
        .Builder()
        .AddContactPoint(host)
        .WithCredentials(userName, password)
        .Build();

    public Task<IMapper> Get()
    {
        return Task.FromResult(new Mapper(cluster.Connect(keySpace)) as IMapper);
    }
}
