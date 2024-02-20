namespace ProperNutritionDiary.Product.Application.Product.RemoveProduct;

using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

public sealed class RemoveProductCommandHandler(
    IProductRepository productRepository,
    ILogger<RemoveProductCommandHandler> logger
) : IRequestHandler<RemoveProductCommand, Result>
{
    private readonly IProductRepository productRepository = productRepository;
    private readonly ILogger<RemoveProductCommandHandler> logger = logger;

    public async Task<Result> Handle(
        RemoveProductCommand request,
        CancellationToken cancellationToken
    )
    {
        return await productRepository
            .GetByIdAsync(new ProductId(request.ProductId))
            .Match(
                OnSuccess(new User(new UserId(request.UserId), request.UserRole)),
                () => Task.FromResult(Result.Failure(ProductErrors.ProductNotFound))
            );
    }

    private Func<Product, Task<Result>> OnSuccess(User user)
    {
        return async (Product product) =>
        {
            var isInFavorite = await productRepository.IsProductInFavoriteList(user.Id, product.Id);

            return await OnSuccess(product, user, isInFavorite);
        };
    }

    private Task<Result> OnSuccess(Product product, User user, bool isInFavorite)
    {
        return Result
            .Check(RemoveProduct(product, user, isInFavorite))
            .Success(() => productRepository.RemoveAsync(product))
            .After(() => logger.LogInformation("Product {@product} updated", product));
    }

    private static Func<Result> RemoveProduct(Product product, User user, bool isInFavorite)
    {
        return () => product.Remove(user, isInFavorite);
    }
}
