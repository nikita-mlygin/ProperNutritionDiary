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

[Collection("main")]
public class ProductRepositoryTest
{
    private readonly ProductId id = new(Guid.NewGuid());
    private const string name = "name";
    private readonly Macronutrients macronutrients = Macronutrients.Create(12, 12, 12, 12).Value;
    private readonly User plainUserCreator = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    private const string newName = "new name";
    private readonly Macronutrients newMacronutrients = Macronutrients.Create(10, 10, 10, 10).Value;

    private readonly IProductRepository productRepository;
    private readonly DateTime createdAt = DateTime.UtcNow;
    private readonly DateTime updatedAt = DateTime.UtcNow.AddMinutes(2);

    public ServiceProvider ServiceProvider { get; set; }

    public ProductRepositoryTest(PersistenceContext context, ITestOutputHelper output)
    {
        context.InjectLogging(output);
        this.ServiceProvider = context.ServiceProvider;

        this.productRepository =
            ServiceProvider.GetService<IProductRepository>() ?? throw new Exception();
    }

    [Fact]
    public async Task CreateAndGetByIdAsync_MustExec()
    {
        var product = Product.Create(id, name, macronutrients, plainUserCreator, createdAt).Value;

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

            resultProduct.Update(newName, newMacronutrients, plainUserCreator, updatedAt);
            await productRepository.UpdateAsync(resultProduct);

            await productRepository.RemoveAsync(resultProduct);
        }
        catch (Exception _)
        {
            throw;
        }
    }

    [Fact]
    public async Task FavoriteListActions_MustExec()
    {
        var id1 = new ProductId(Guid.NewGuid());
        var id2 = new ProductId(Guid.NewGuid());
        var id3 = new ProductId(Guid.NewGuid());

        var name1 = "name1";
        var name2 = "name2";
        var name3 = "name3";

        var systemCreator = new User(new UserId(Guid.NewGuid()), UserRole.Admin);

        var firstProduct = Product
            .Create(id1, name1, macronutrients, plainUserCreator, createdAt)
            .Value;
        var secondProduct = Product
            .Create(id2, name2, macronutrients, systemCreator, createdAt)
            .Value;
        var thirdProduct = Product
            .Create(id3, name3, macronutrients, plainUserCreator, createdAt)
            .Value;

        await productRepository.CreateAsync(firstProduct);
        await productRepository.CreateAsync(secondProduct);
        await productRepository.CreateAsync(thirdProduct);

        await productRepository.AddProductToFavoriteList(
            plainUserCreator.Id,
            firstProduct.Id,
            DateTime.UtcNow
        );

        await productRepository.AddProductToFavoriteList(
            plainUserCreator.Id,
            secondProduct.Id,
            DateTime.UtcNow
        );

        await productRepository.AddProductToFavoriteList(
            plainUserCreator.Id,
            thirdProduct.Id,
            DateTime.UtcNow
        );

        (await productRepository.GetFavoriteProductListAsync(plainUserCreator.Id))
            .Should()
            .Contain(firstProduct)
            .And.Contain(secondProduct)
            .And.Contain(thirdProduct);
    }

    [Fact]
    public async Task IsInFavoriteList_MustReturnFalse_WhenIsNotInFavoriteList()
    {
        var product = Product.Create(id, name, macronutrients, plainUserCreator, createdAt).Value;

        await productRepository.CreateAsync(product);

        (await productRepository.IsProductInFavoriteList(plainUserCreator.Id, product.Id))
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task IsInFavoriteList_MustReturnTrue_WhenIsInFavoriteList()
    {
        var product = Product.Create(id, name, macronutrients, plainUserCreator, createdAt).Value;

        await productRepository.CreateAsync(product);

        await productRepository.AddProductToFavoriteList(
            plainUserCreator.Id,
            product.Id,
            createdAt
        );

        (await productRepository.IsProductInFavoriteList(plainUserCreator.Id, product.Id))
            .Should()
            .BeTrue();
    }
}
