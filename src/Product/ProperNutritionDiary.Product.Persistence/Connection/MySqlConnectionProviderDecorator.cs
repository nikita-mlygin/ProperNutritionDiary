using DomainDesignLib.Persistence.Repository;
using DomainDesignLib.Persistence.Repository.Hooks;
using Microsoft.Extensions.Logging;

namespace ProperNutritionDiary.Product.Persistence.Connection;

public class MySqlConnectionProviderDecorator(
    IConnectionProvider connectionProvider,
    ILogger<MySqlConnectionProviderDecorator> logger,
    DbLoggingConfiguration configuration
)
    : ConnectionProviderDecorator(connectionProvider, logger, configuration),
        ISqlConnectionProvider { }
