namespace ProperNutritionDiary.Test.Product.Test.Domain;

using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Event;
using FluentAssertions;
using NSubstitute;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Favorite;
using ProperNutritionDiary.Product.Domain.Product.Favorite.Add;
using ProperNutritionDiary.Product.Domain.User;

public class ProductFavoriteServiceTest
{
    private readonly ProductFavoriteService productFavoriteService;
    private readonly IProductRepository productRepository;
    private readonly IEventDispatcher eventDispatcher;

    private readonly User adminUser = new(new UserId(Guid.NewGuid()), UserRole.Admin);
    private readonly User plainUser = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    private readonly Product testProduct;

    public ProductFavoriteServiceTest()
    {
        productRepository = Substitute.For<IProductRepository>();
        productRepository
            .AddProductToFavoriteListAsync(
                Arg.Any<UserId>(),
                Arg.Any<ProductId>(),
                Arg.Any<DateTime>()
            )
            .Returns(Task.CompletedTask);

        eventDispatcher = Substitute.For<IEventDispatcher>();

        eventDispatcher.AddEvent(Arg.Any<DomainEvent>()).Returns(Task.CompletedTask);

        productFavoriteService = new ProductFavoriteService(productRepository, eventDispatcher);

        testProduct = Product.FromSnapshot(
            new ProductSnapshot()
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Macronutrients = new MacronutrientsSnapshot(10, 2, 3, 4),
                Owner = plainUser.Id.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        );
    }

    [Fact]
    public async Task AddToFavoriteList_ShouldFailed_WhenUserIsAdmin()
    {
        var res = await productFavoriteService.AddProductToFavoriteList(
            adminUser,
            testProduct,
            false,
            DateTime.UtcNow
        );

        res.IsFailure.Should().BeTrue();
        res.Error.Should().Be(ProductErrors.AdminUserNotAllowedAddToFavoriteList);
    }

    [Fact]
    public async Task AddToFavoriteList_ShouldFailed_WhenProductAlreadyInFavoriteList()
    {
        var res = await productFavoriteService.AddProductToFavoriteList(
            plainUser,
            testProduct,
            true,
            DateTime.UtcNow
        );

        res.IsFailure.Should().BeTrue();
        res.Error.Should().Be(ProductErrors.ProductAlreadyInFavoriteList);
    }

    [Fact]
    public async Task AddToFavoriteList_ShouldSuccess_WhenOk()
    {
        var timeAdded = DateTime.UtcNow;

        var res = await productFavoriteService.AddProductToFavoriteList(
            plainUser,
            testProduct,
            false,
            timeAdded
        );

        res.IsFailure.Should().BeFalse();

        await eventDispatcher
            .Received(1)
            .AddEvent(
                Arg.Is<ProductAddedToFavorite>(e =>
                    e.Product == testProduct.Id && e.User == plainUser.Id
                )
            );

        await productRepository
            .Received(1)
            .AddProductToFavoriteListAsync(
                Arg.Is<UserId>(id => id == plainUser.Id),
                Arg.Is<ProductId>(pr => pr == testProduct.Id),
                Arg.Is<DateTime>(date => date == timeAdded)
            );
    }
}
