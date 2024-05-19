namespace ProperNutritionDiary.Product.Application.Product.Add;

using DomainDesignLib.Abstractions.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ILogger<CreateProductCommandHandler> logger
) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository productRepository = productRepository;
    private readonly ILogger<CreateProductCommandHandler> logger = logger;

    public Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = new User(new UserId(request.UserId ?? Guid.Empty), request.UserRole);
        var id = Guid.NewGuid();
        Result<Macronutrients> macronutrients = null!;
        Result<Product> product = null!;

        return Result
            .Check(
                () =>
                    macronutrients = Macronutrients.Create(
                        request.Calories,
                        request.Proteins,
                        request.Fats,
                        request.Carbohydrates
                    )
            )
            .Check(
                () =>
                    product = Product.Create(
                        new ProductId(id),
                        request.ProductName,
                        macronutrients.Value,
                        user,
                        DateTime.UtcNow
                    )
            )
            .Success(() => OnSuccess(product.Value)());
    }

    private Func<Task<Guid>> OnSuccess(Product product)
    {
        return async () =>
        {
            await productRepository.CreateAsync(product);
            logger.LogInformation("Product {@product} entity created.", product);

            return product.Id.Value;
        };
    }
}
