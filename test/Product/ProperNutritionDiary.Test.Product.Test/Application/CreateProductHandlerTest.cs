namespace ProperNutritionDiary.Test.Product.Test.Application;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProperNutritionDiary.Product.Application.Product.Add;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public class CreateProductHandlerTest
{
    private readonly IProductRepository productSummaryRepository;
    private readonly ILogger<CreateProductCommandHandler> logger;
    private readonly CreateProductCommand command;

    private const string productName = "productName";
    private readonly User user = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);
    private readonly Macronutrients macronutrients = Macronutrients.Create(12, 12, 1, 1).Value;

    public CreateProductHandlerTest()
    {
        this.productSummaryRepository = Substitute.For<IProductRepository>();
        this.logger = Substitute.For<ILogger<CreateProductCommandHandler>>();
        command = new CreateProductCommand(
            user.Id.Value,
            user.Role,
            productName,
            macronutrients.Calories,
            macronutrients.Proteins,
            macronutrients.Fats,
            macronutrients.Carbohydrates
        );
    }

    [Fact]
    public async Task Handle_MustReturnFailed_WhenMacronutrientsHasError()
    {
        CreateProductCommandHandler handler = new(productSummaryRepository, logger);

        var res = await handler.Handle(
            new CreateProductCommand(user.Id.Value, UserRole.PlainUser, productName, -1, 0, 0, 0),
            CancellationToken.None
        );

        res.IsFailure.Should().BeTrue();
        res.Error.Should().Be(MacronutrientsErrors.ValueLessZero);
    }

    [Fact]
    public async Task Handle_MustReturnFailed_WhenProductHasError()
    {
        CreateProductCommandHandler handler = new(productSummaryRepository, logger);

        var res = await handler.Handle(
            new CreateProductCommand(user.Id.Value, UserRole.PlainUser, "", 1, 0, 0, 0),
            CancellationToken.None
        );

        res.IsFailure.Should().BeTrue();
        res.Error.Should().Be(ProductErrors.NameIsNullOrEmpty);
    }

    [Fact]
    public async Task Handle_MustExec()
    {
        CreateProductCommandHandler handler = new(productSummaryRepository, logger);

        var res = await handler.Handle(command, CancellationToken.None);

        res.IsSuccess.Should().BeTrue();
        res.Value.Should().NotBeEmpty();

        await productSummaryRepository
            .Received(1)
            .CreateAsync(
                Arg.Is<Product>(product =>
                    product.Name == productName
                    && !product.Owner.IsSystem
                    && product.Owner.Owner == user.Id
                    && product.Macronutrients == macronutrients
                )
            );
    }
}
