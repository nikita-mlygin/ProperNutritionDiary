using ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;
using ProperNutritionDiary.Product.Persistence.Product;

namespace ProperNutritionDiary.Test.Product.Test.Persistence;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence;

public class ProductRepositoryTest
{
    private Product product = Product
        .Create(
            new ProductId(Guid.NewGuid()),
            "name",
            Macronutrients.Create(12, 12, 12, 12).Value,
            new User(new UserId(Guid.NewGuid()), UserRole.PlainUser)
        )
        .Value;
    private readonly ProductRepository productRepository;
    private readonly IConnectionProvider connectionProvider;
    private readonly IServiceCollection services;
    private const string connectionString = "server=localhost;uid=user;pwd=secret;database=dev";

    public ProductRepositoryTest()
    {
        connectionProvider = new MySqlConnectionProvider(connectionString);
        productRepository = new ProductRepository(connectionProvider);
        services = Substitute.For<IServiceCollection>();
        services.AddPersistence();
    }

    [Fact]
    public async Task CreateAsync_MustExec()
    {
        Exception? exception = null;

        try
        {
            await productRepository.CreateAsync(product);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        exception.Should().BeNull();
    }
}
