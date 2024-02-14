namespace ProperNutritionDiary.Product.Domain.Product.Favorite;

using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Event;
using DomainDesignLib.Abstractions.Result;
using ProperNutritionDiary.Product.Domain.Product.Favorite.Add;
using ProperNutritionDiary.Product.Domain.User;

public class ProductFavoriteService(
    IProductRepository productRepository,
    IEventDispatcher eventDispatcher
) : IProductFavoriteService
{
    private readonly IProductRepository productRepository = productRepository;
    private readonly IEventDispatcher eventDispatcher = eventDispatcher;

    public Task<Result> AddProductToFavoriteList(
        User user,
        Product product,
        bool isProductInFavoriteList,
        DateTime timeAdded
    )
    {
        return Result
            .Check(
                user.Role,
                role => role == UserRole.Admin,
                ProductErrors.AdminUserNotAllowedAddToFavoriteList
            )
            .Check(isProductInFavoriteList, ProductErrors.ProductAlreadyInFavoriteList)
            .Success(AddProductToFavoriteList(user, product, timeAdded))
            .After(DispatchEvent(new ProductAddedToFavorite(Guid.NewGuid(), user.Id, product.Id)));
    }

    public Task<Result> RemoveProductFromFavoriteList(
        User user,
        Product product,
        bool isProductInFavoriteList
    )
    {
        return Result
            .Check(isProductInFavoriteList, ProductErrors.ProductNotInFavoriteList)
            .Success(RemoveProductFromFavoriteList(user, product));
    }

    private Func<Task> AddProductToFavoriteList(User user, Product product, DateTime timeAdded) =>
        async () =>
            await productRepository.AddProductToFavoriteListAsync(user.Id, product.Id, timeAdded);

    private Func<Task> RemoveProductFromFavoriteList(User user, Product product) =>
        async () => await productRepository.RemoveProductFromFavoriteListAsync(user.Id, product.Id);

    private Func<Task> DispatchEvent(DomainEvent e)
    {
        return () => eventDispatcher.AddEvent(e);
    }
}
