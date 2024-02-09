using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product.Create;
using ProperNutritionDiary.Product.Domain.Product.Remove;
using ProperNutritionDiary.Product.Domain.Product.Update;
using ProperNutritionDiary.Product.Domain.User;

public class Product : Entity<ProductId>
{
    private Product(ProductId id, string name, Macronutrients macronutrients, ProductOwner owner)
        : base(id)
    {
        Name = name;
        Macronutrients = macronutrients;
        Owner = owner;
    }

    public static Result<Product> Create(
        ProductId id,
        string name,
        Macronutrients macronutrients,
        User creator
    )
    {
        return Result
            .Check(name, string.IsNullOrEmpty, ProductErrors.NameIsNullOrEmpty)
            .Check(creator, creator => creator.Role == UserRole.Guest, ProductErrors.CreatorIsGuest)
            .Success(() =>
            {
                var product = new Product(
                    new ProductId(Guid.NewGuid()),
                    name,
                    macronutrients,
                    creator.Role == UserRole.Admin
                        ? ProductOwner.BySystem()
                        : ProductOwner.ByUser(creator.Id)
                );

                product.Raise(new ProductCreated(Guid.NewGuid(), product));

                return product;
            });
    }

    public Result Update(string name, Macronutrients macronutrients, User updater)
    {
        return Result
            .Check(name, string.IsNullOrEmpty, ProductErrors.NameIsNullOrEmpty)
            .Check(updater, CheckUpdater, ProductErrors.UpdateNotAllowedToNoOwner)
            .Success(() =>
            {
                var oldName = Name;
                var oldMacronutrients = Macronutrients;

                Name = name;
                Macronutrients = macronutrients;

                Raise(new ProductUpdated(Guid.NewGuid(), oldName, oldMacronutrients, this));
            });
    }

    public Result Remove(User remover)
    {
        return Result
            .Check(remover, CheckRemover, ProductErrors.RemoveNotAllowedToNoOwner)
            .Success(() =>
            {
                Raise(new ProductRemoved(Guid.NewGuid(), this));
            });
    }

    public string Name { get; private set; }
    public Macronutrients Macronutrients { get; private set; }
    public ProductOwner Owner { get; private set; }

    private bool CheckUpdater(User updater) =>
        (Owner.IsSystem && updater.Role == UserRole.Admin) || (updater.Id != this.Owner.Owner);

    private bool CheckRemover(User remover) =>
        (Owner.IsSystem && remover.Role == UserRole.Admin) || (Owner.Owner != remover.Id);

    public static Product FromSnapshot(ProductSnapshot product)
    {
        throw new NotImplementedException();
    }
}
