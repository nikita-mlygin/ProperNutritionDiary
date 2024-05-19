using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Product;

public static class ProductErrors
{
    public static readonly Error NameIsNullOrEmpty =
        new(
            $"{nameof(Product)}.{nameof(NameIsNullOrEmpty)}",
            "Product name must be not not null or empty"
        );
    public static readonly Error CreatorIsGuest =
        new(
            $"{nameof(Product)}.{nameof(CreatorIsGuest)}",
            "Product create can only authorize user"
        );
    public static readonly Error UpdateNotAllowedToNoOwner =
        new(
            $"{nameof(Product)}.{nameof(UpdateNotAllowedToNoOwner)}",
            "Update product can only owner"
        );
    public static readonly Error RemoveNotAllowedToNoOwner =
        new(
            $"{nameof(Product)}.{nameof(RemoveNotAllowedToNoOwner)}",
            "Remove product can only owner"
        );

    public static readonly Error RemoveFromExternalSource =
        new(
            $"{nameof(Product)}.{nameof(RemoveFromExternalSource)}",
            "Can't remove product from external source"
        );

    public static readonly Error RemoveNotAllowedWhenInFavoriteList =
        new(
            $"{nameof(Product)}.{nameof(RemoveNotAllowedWhenInFavoriteList)}",
            "Can't remove if product in any favorite list"
        );

    public static readonly Error AdminUserNotAllowedAddToFavoriteList =
        new(
            $"{nameof(Product)}.{nameof(AdminUserNotAllowedAddToFavoriteList)}",
            "Admin user can't add product in the favorite list."
        );

    public static readonly Error ProductAlreadyInFavoriteList =
        new(
            $"{nameof(Product)}.{nameof(ProductAlreadyInFavoriteList)}",
            "Product can't be added in favorite list if it already in it"
        );

    public static readonly Error ProductNotInFavoriteList =
        new(
            $"{nameof(Product)}.{nameof(ProductNotInFavoriteList)}",
            "Product can't be removed from favorite list if it not in"
        );

    public static readonly Error ProductNotFound =
        new($"{nameof(Product)}.{nameof(ProductNotFound)}", "Product with this id is not found.");
}
