namespace ProperNutritionDiary.Test.Product.Test.Application;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProperNutritionDiary.Product.Application.Product.RemoveProduct;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public class RemoveProductHandlerTest
{
    private readonly IProductRepository productRepository;
    private readonly ILogger<RemoveProductCommandHandler> logger;
    private readonly RemoveProductCommandHandler handler;
    private readonly Product product;

    private readonly RemoveProductCommand command;

    private User user = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    public RemoveProductHandlerTest()
    {
        productRepository = Substitute.For<IProductRepository>();
        logger = Substitute.For<ILogger<RemoveProductCommandHandler>>();

        handler = new RemoveProductCommandHandler(productRepository, logger);

        product = Product
            .Create(
                new ProductId(Guid.NewGuid()),
                "name1",
                Macronutrients.Create(0, 1, 2, 3).Value,
                user,
                DateTime.UtcNow
            )
            .Value;

        command = new RemoveProductCommand(user.Id.Value, user.Role, product.Id.Value);
    }

    [Fact]
    public async Task Handle_MustReturnFailed_WhenErrInCommand()
    {
        var res = await handler.Handle(
            new RemoveProductCommand(user.Id.Value, user.Role, product.Id.Value),
            CancellationToken.None
        );

        res.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MustExec()
    {
        productRepository.GetByIdAsync(Arg.Is(product.Id)).Returns(product);

        var res = await handler.Handle(command, CancellationToken.None);

        res.IsSuccess.Should().BeTrue();

        await productRepository.Received(1).RemoveAsync(Arg.Is(product));
    }
}
