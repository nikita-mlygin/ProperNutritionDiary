using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.User;

interface IProductDomainService
{
    public Task<Result> UpdateViewers(UserId viewer, ProductId product);
    public Task<Result> UpdateAdded(UserId addUser, ProductId product);
    public Task<Result> AddProductToFavoriteList(User user, ProductId product, DateTime timeAdded);
    public Task<Result> RemoveProductFromFavoriteList(UserId user, ProductId product);
}

public record FavoriteProductLineItem(UserId User, ProductId Product, DateTime TimeAdded);
