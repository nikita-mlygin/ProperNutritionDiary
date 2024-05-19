using DomainDesignLib.Persistence.Repository.Hooks;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Persistence;
using Serilog;
using Xunit.Abstractions;

namespace ProperNutritionDiary.Test.Product.Test.Persistence;

public class PersistenceContext
{
    public ServiceProvider provider;

    private const string host = "localhost";
    private const string keySpace = "product";
    private const string name = "user";

    public ServiceProvider ServiceProvider
    {
        get => Services.BuildServiceProvider();
    }
    protected IServiceCollection Services { get; set; }

    public PersistenceContext()
    {
        Services = new ServiceCollection();

        var mysqlPassword = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__PRODUCT_MYSQL_PASSWORD"
        );
        var cassandraPassword = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__PRODUCT_CASSANDRA_PASSWORD"
        );
        var usdaApiKey = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__PRODUCT_USDA_API_KEY"
        );

        Services.AddPersistence(
            $"server=localhost;uid=user;pwd={mysqlPassword ?? throw new Exception()};database=dev",
            host,
            keySpace,
            name,
            cassandraPassword ?? throw new Exception(),
            usdaApiKey ?? throw new Exception()
        );
    }

    public void InjectLogging(ITestOutputHelper output)
    {
        #region loggingConfig
        Services.AddLogging(
            (builder) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.TestOutput(output)
                    .CreateLogger();

                builder.AddSerilog();

                builder.Services.AddSingleton<ILoggerProvider>(
                    serviceProvider => new XUnitLoggerProvider(output)
                );
            }
        );

        Services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(_ =>
            XUnitLogger.CreateLogger(output)
        );

        Services.AddSingleton(_ =>
        {
            var factory = new LoggerFactory();
            factory.AddSerilog(Log.Logger);
            return factory;
        });

        Services.AddSingleton(_ => DbLoggingConfiguration.Default);
        #endregion
    }
}
