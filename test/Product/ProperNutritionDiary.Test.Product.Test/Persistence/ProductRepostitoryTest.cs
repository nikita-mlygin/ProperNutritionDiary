namespace ProperNutritionDiary.Test.Product.Test.Persistence;

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;
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

        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator.Id,
            firstProduct.Id,
            DateTime.UtcNow
        );

        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator.Id,
            secondProduct.Id,
            DateTime.UtcNow
        );

        await productRepository.AddProductToFavoriteListAsync(
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

        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator.Id,
            product.Id,
            createdAt
        );

        (await productRepository.IsProductInFavoriteList(plainUserCreator.Id, product.Id))
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task GetUserWhichFavoriteListContainsProduct_MustReturnUsers()
    {
        var id1 = new ProductId(Guid.NewGuid());
        var id2 = new ProductId(Guid.NewGuid());

        var name1 = "Name1";
        var name2 = "Name2";

        var plainUserCreator2 = new User(new UserId(Guid.NewGuid()), UserRole.PlainUser);
        var systemCreator = new User(new UserId(Guid.NewGuid()), UserRole.Admin);

        var product1 = Product.Create(id1, name1, macronutrients, systemCreator, createdAt).Value;
        var product2 = Product.Create(id2, name2, macronutrients, systemCreator, createdAt).Value;

        await productRepository.CreateAsync(product1);
        await productRepository.CreateAsync(product2);

        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator.Id,
            product1.Id,
            DateTime.UtcNow
        );
        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator2.Id,
            product1.Id,
            DateTime.UtcNow
        );

        await productRepository.AddProductToFavoriteListAsync(
            plainUserCreator.Id,
            product2.Id,
            DateTime.UtcNow
        );

        var users1 = await productRepository.GetUserWhichFavoriteListContainsProduct(product1);
        var users2 = await productRepository.GetUserWhichFavoriteListContainsProduct(product2);

        users1.Count().Should().Be(2);
        users2.Count().Should().Be(1);

        users1.FirstOrDefault(x => x == plainUserCreator.Id).Should().NotBeNull();
        users1.FirstOrDefault(x => x == plainUserCreator2.Id).Should().NotBeNull();

        users2.FirstOrDefault(x => x == plainUserCreator.Id).Should().NotBeNull();
    }
}
