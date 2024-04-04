namespace ProperNutritionDiary.Product.Application.Product.Update;

using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ILogger<UpdateProductCommandHandler> logger
) : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository productRepository = productRepository;
    private readonly ILogger<UpdateProductCommandHandler> logger = logger;

    public Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return productRepository
            .GetByIdAsync(new ProductId(request.ProductId))
            .Match(
                OnSuccess(request, new User(new UserId(request.UserId), request.UserRole)),
                () => Task.FromResult(Result.Failure(ProductErrors.ProductNotFound))
            );
    }

    private Func<Product, Task<Result>> OnSuccess(UpdateProductCommand request, User user)
    {
        return (Product product) =>
        {
            Result<Macronutrients> macronutrients = null!;

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
                .Check(() => UpdateProduct(product, request.NewName, macronutrients.Value, user)())
                .Success(() => productRepository.UpdateAsync(product))
                .After(() => logger.LogInformation("Product {@product} updated", product));
        };
    }

    private static Func<Result> UpdateProduct(
        Product product,
        string newName,
        Macronutrients macronutrients,
        User user
    )
    {
        return () => product.Update(newName, macronutrients, user, DateTime.UtcNow);
    }
}
