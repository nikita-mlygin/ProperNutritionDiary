namespace ProperNutritionDiary.Test.Product.Test.Application;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using ProperNutritionDiary.Product.Application.Product.Update;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public class UpdateProductHandlerTest
{
    private readonly IProductRepository productRepository;
    private readonly ILogger<UpdateProductCommandHandler> logger;
    private UpdateProductCommandHandler handler;

    private const string newName = "newName";
    private readonly Macronutrients newMacronutrients = Macronutrients.Create(12, 12, 12, 12).Value;
    private readonly User user = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);
    private Product product;

    private UpdateProductCommand command;

    public UpdateProductHandlerTest()
    {
        productRepository = Substitute.For<IProductRepository>();
        logger = Substitute.For<ILogger<UpdateProductCommandHandler>>();
        handler = new UpdateProductCommandHandler(productRepository, logger);

        product = Product
            .Create(
                new ProductId(Guid.NewGuid()),
                "name",
                Macronutrients.Create(1, 1, 1, 1).Value,
                user,
                DateTime.Now
            )
            .Value;

        command = new UpdateProductCommand(
            user.Id.Value,
            user.Role,
            product.Id.Value,
            newName,
            newMacronutrients.Calories,
            newMacronutrients.Proteins,
            newMacronutrients.Fats,
            newMacronutrients.Carbohydrates
        );
    }

    [Fact]
    public async Task Handle_MustReturnFailed_WhenErrInCommand()
    {
        var res = await handler.Handle(
            new UpdateProductCommand(user.Id.Value, user.Role, product.Id.Value, "", 0, 0, 0, 0),
            CancellationToken.None
        );

        res.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MustExec()
    {
        productRepository.GetByIdAsync(Arg.Is<ProductId>(v => v == product.Id)).Returns(product);

        var res = await handler.Handle(command, CancellationToken.None);

        res.IsSuccess.Should().BeTrue();

        await productRepository.Received(1).GetByIdAsync(Arg.Is(product.Id));
        await productRepository.Received(1).UpdateAsync(Arg.Is(product));
    }
}
