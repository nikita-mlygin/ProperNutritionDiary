using DomainDesignLib.Abstractions.Event;
using FluentAssertions;
using NSubstitute;
using ProperNutritionDiary.Product.Application.Product.Get.ById;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Test.Product.Test.Application;

public class GetProductByIdHandlerTest
{
    private readonly IProductSummaryRepository productRepository;
    private readonly IEventDispatcher eventDispatcher;
    private readonly GetProductByIdQueryHandler handler;
    private readonly ProductSummary productSummary;
    private readonly GetProductByIdQuery query;
    private readonly User user = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    public GetProductByIdHandlerTest()
    {
        productRepository = Substitute.For<IProductSummaryRepository>();
        eventDispatcher = Substitute.For<IEventDispatcher>();
        handler = new GetProductByIdQueryHandler(productRepository, eventDispatcher);
        productSummary = new ProductSummary(
            new ProductId(Guid.NewGuid()),
            "name",
            Macronutrients.Create(0, 0, 0, 0).Value,
            ProductOwner.ByUser(user.Id),
            10000,
            123
        );
        query = new GetProductByIdQuery(productSummary.Id.Value, user.Id.Value, user.Role);
    }

    [Fact]
    public async Task Handle_MustExec()
    {
        productRepository.GetById(Arg.Is(productSummary.Id)).Returns(productSummary);

        var res = await handler.Handle(query, CancellationToken.None);

        res.Should().Be(productSummary);
        await eventDispatcher
            .Received(1)
            .AddEvent(
                Arg.Is<ProductReceived>(e =>
                    e.ReceivedProduct == productSummary.Id && e.User == user.Id
                )
            );
    }
}
