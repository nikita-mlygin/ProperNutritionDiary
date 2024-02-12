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

    private const string connectionString = "server=localhost;uid=user;pwd=secret;database=dev";
    private const string host = "localhost";
    private const string keySpace = "product";
    private const string name = "user";
    private const string password = "secret";

    public ServiceProvider ServiceProvider
    {
        get => Services.BuildServiceProvider();
    }
    protected IServiceCollection Services { get; set; }

    public PersistenceContext()
    {
        Services = new ServiceCollection();

        Services.AddPersistence(connectionString, host, keySpace, name, password);
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
