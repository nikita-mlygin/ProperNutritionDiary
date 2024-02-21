using DomainDesignLib.Abstractions.Event;
using FluentAssertions;
using NSubstitute;
using ProperNutritionDiary.Product.Application.Product.Get.ById;
using ProperNutritionDiary.Product.Application.Product.Get.List;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Test.Product.Test.Application;

public class GetProductListHandlerTest
{
    private readonly IProductSummaryRepository productRepository;
    private readonly IEventDispatcher eventDispatcher;
    private readonly GetProductListHandler handler;
    private readonly ProductSummary productSummary;
    private readonly GetProductList query;
    private readonly User user = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    public GetProductListHandlerTest()
    {
        productRepository = Substitute.For<IProductSummaryRepository>();
        eventDispatcher = Substitute.For<IEventDispatcher>();
        handler = new GetProductListHandler(productRepository);
        productSummary = new ProductSummary(
            new ProductId(Guid.NewGuid()),
            "name",
            Macronutrients.Create(0, 0, 0, 0).Value,
            ProductOwner.ByUser(user.Id),
            10000,
            123
        );
        query = new GetProductList(user.Id.Value, user.Role, "aboba", productSummary.Id.Value);
    }

    [Fact]
    public async Task Handle_MustExec()
    {
        productRepository
            .GetProductList(Arg.Any<string>(), Arg.Any<ProductId>())
            .Returns(
                Task.FromResult(
                    new List<ProductListSummary>()
                    {
                        new(productSummary.Id, productSummary.Name, productSummary.Owner)
                    }
                )
            );

        var res = await handler.Handle(query, CancellationToken.None);

        await productRepository
            .Received(1)
            .GetProductList(
                Arg.Is(query.Query),
                Arg.Is(
                    query.LastProduct is not null ? new ProductId((Guid)query.LastProduct) : null
                )
            );

        res.Should().NotBeNullOrEmpty();
    }
}
