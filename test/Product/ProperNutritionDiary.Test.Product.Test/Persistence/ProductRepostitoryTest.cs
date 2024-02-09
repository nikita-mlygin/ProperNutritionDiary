using ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;

namespace ProperNutritionDiary.Test.Product.Test.Persistence;

using System;
using DomainDesignLib.Persistence.Repository;
using DomainDesignLib.Persistence.Repository.Hooks;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence;
using Serilog;
using Xunit.Abstractions;

public class ProductRepositoryTest
{
    private readonly ProductId id = new(Guid.NewGuid());
    private const string name = "name";
    private readonly Macronutrients macronutrients = Macronutrients.Create(12, 12, 12, 12).Value;
    private readonly User plainUserCreator = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    private const string newName = "new name";
    private readonly Macronutrients newMacronutrients = Macronutrients.Create(10, 10, 10, 10).Value;

    private readonly IProductRepository productRepository;
    private const string connectionString = "server=localhost;uid=user;pwd=secret;database=dev";

    public ServiceProvider ServiceProvider { get; set; } = default!;

    protected IServiceCollection Services { get; set; }

    public ProductRepositoryTest(ITestOutputHelper output)
    {
        Services = new ServiceCollection();
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

        Services.AddScoped<IConnectionProvider, MySqlConnectionProvider>(
            _ => new MySqlConnectionProvider(connectionString)
        );
        Services.AddSingleton((_) => DbLoggingConfiguration.Default);
        Services.Decorate<IConnectionProvider, ConnectionProviderDecorator>();

        Services.AddPersistence();

        ServiceProvider = Services.BuildServiceProvider();

        this.productRepository = ServiceProvider.GetService<IProductRepository>();
    }

    [Fact]
    public async Task CreateAndGetByIdAsync_MustExec()
    {
        var product = Product.Create(id, name, macronutrients, plainUserCreator).Value;

        try
        {
            await productRepository.CreateAsync(product);

            var resultProduct = await productRepository.GetByIdAsync(product.Id);
            resultProduct.Should().NotBeNull();
            resultProduct!.Id.Should().Be(id);
            resultProduct.Name.Should().Be(name);
            resultProduct.Macronutrients.Should().Be(macronutrients);
            resultProduct.Owner.Owner.Should().Be(plainUserCreator.Id);

            var productList = await productRepository.GetAll();
            productList.Should().NotBeEmpty();

            resultProduct.Update(newName, newMacronutrients, plainUserCreator);
            await productRepository.UpdateAsync(resultProduct);

            await productRepository.RemoveAsync(resultProduct);
        }
        catch (Exception _)
        {
            throw;
        }
    }
}
