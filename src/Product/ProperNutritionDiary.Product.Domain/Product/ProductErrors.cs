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
}
