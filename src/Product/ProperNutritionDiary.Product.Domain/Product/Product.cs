using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Product;

using DomainDesignLib.Abstractions.Entity;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product.Create;
using ProperNutritionDiary.Product.Domain.Product.Remove;
using ProperNutritionDiary.Product.Domain.Product.Update;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

#pragma warning disable S3453

public class Product : Entity<ProductId>, IAuditable
{
    private Product(
        ProductId id,
        string name,
        Macronutrients macronutrients,
        ProductOwner? owner,
        DateTime createdAt,
        DateTime? updatedAt,
        ExternalSourceIdentity? externalSource = null
    )
        : base(id)
    {
        Name = name;
        Macronutrients = macronutrients;
        Owner = owner;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ExternalSource = externalSource;
    }

    public static Result<Product> Create(
        ProductId id,
        string name,
        Macronutrients macronutrients,
        User creator,
        DateTime createdAt
    )
    {
        return Result
            .Check(name, string.IsNullOrEmpty, ProductErrors.NameIsNullOrEmpty)
            .Check(creator, creator => creator.Role == UserRole.Guest, ProductErrors.CreatorIsGuest)
            .Success(() =>
            {
                var product = new Product(
                    id,
                    name,
                    macronutrients,
                    creator.Role is UserRole.Admin or UserRole.App
                        ? ProductOwner.BySystem()
                        : ProductOwner.ByUser(creator.Id),
                    createdAt,
                    null
                );

                product.Raise(new ProductCreated(Guid.NewGuid(), product));

                return product;
            });
    }

    public Result Update(
        string name,
        Macronutrients macronutrients,
        User updater,
        DateTime updateTime
    )
    {
        Owner ??=
            (updater.Role == UserRole.Admin || updater.Role == UserRole.App)
                ? ProductOwner.BySystem()
                : ProductOwner.ByUser(updater.Id);

        return Result
            .Check(name, string.IsNullOrEmpty, ProductErrors.NameIsNullOrEmpty)
            .Check(updater, CheckUpdater, ProductErrors.UpdateNotAllowedToNoOwner)
            .Success(() =>
            {
                var oldName = Name;
                var oldMacronutrients = Macronutrients;

                Name = name;
                Macronutrients = macronutrients;
                UpdatedAt = updateTime;

                Raise(new ProductUpdated(Guid.NewGuid(), oldName, oldMacronutrients, this));
            });
    }

    public Result Remove(User remover, bool isInFavoriteList)
    {
        return Result
            .Check(Owner is null, ProductErrors.RemoveFromExternalSource)
            .Check(remover, CheckRemover, ProductErrors.RemoveNotAllowedToNoOwner)
            .Check(isInFavoriteList, (v) => v, ProductErrors.RemoveNotAllowedWhenInFavoriteList)
            .Success(() =>
            {
                Raise(new ProductRemoved(Guid.NewGuid(), this));
            });
    }

    public string Name { get; private set; }

    public Macronutrients Macronutrients { get; private set; }

    public ProductOwner? Owner { get; private set; }

    public ExternalSourceIdentity? ExternalSource { get; }

    public bool IsExternalSourceProduct
    {
        get => Owner is null;
    }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private bool CheckUpdater(User updater) =>
        Owner is null
        || (Owner.IsSystem && updater.Role == UserRole.PlainUser)
        || (updater.Id != this.Owner.Owner);

    private bool CheckRemover(User remover) =>
        Owner is null
        || (Owner.IsSystem && remover.Role == UserRole.PlainUser)
        || (Owner.Owner != remover.Id);

    public static Product FromSnapshot(ProductSnapshot product)
    {
        ProductOwner? owner = null;
        ExternalSourceIdentity? externalSourceIdentity = null;

        if (!product.FromExternalSource)
            owner = product.Owner is null
                ? ProductOwner.BySystem()
                : ProductOwner.ByUser(new UserId((Guid)product.Owner));

        if (product.FromExternalSource)
            externalSourceIdentity = ExternalSourceIdentity.Create(
                product.ExternalSourceType!.Value,
                product.ExternalSource!
            );

        return new Product(
            new ProductId(product.Id),
            product.Name,
            Macronutrients.FromSnapshot(product.Macronutrients),
            owner,
            product.CreatedAt,
            product.UpdatedAt,
            externalSourceIdentity
        );
    }

    public ProductSnapshot ToSnapshot()
    {
        return new ProductSnapshot()
        {
            Id = this.Id.Value,
            Name = this.Name,
            FromExternalSource = this.Owner is null,
            Owner = this.Owner is null ? null : this.Owner.Owner?.Value,
            Macronutrients = this.Macronutrients.ToSnapshot(),
            CreatedAt = this.CreatedAt,
            UpdatedAt = this.UpdatedAt,
        };
    }
}

#pragma warning restore S3453
