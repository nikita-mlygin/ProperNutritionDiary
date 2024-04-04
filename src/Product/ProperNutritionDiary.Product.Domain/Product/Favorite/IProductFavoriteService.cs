using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Product.Favorite;

using ProperNutritionDiary.Product.Domain.User;

interface IProductFavoriteService
{
    public Task<Result> AddProductToFavoriteList(
        User user,
        Product product,
        bool isProductInFavoriteList,
        DateTime timeAdded
    );
    public Task<Result> RemoveProductFromFavoriteList(
        User user,
        Product product,
        bool isProductInFavoriteList
    );
}
