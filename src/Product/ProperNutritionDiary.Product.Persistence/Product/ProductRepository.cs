namespace ProperNutritionDiary.Product.Persistence.Product;

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

internal class ProductRepository(IConnectionProvider connectionProvider) : IProductRepository
{
    private readonly IConnectionProvider connectionProvider = connectionProvider;

    public Task AddProductToFavoriteList(FavoriteProductLineItem favoriteProductListItem)
    {
        throw new NotImplementedException();
    }

    public async Task CreateAsync(Product entity)
    {
        var ownerColumDef = entity.Owner.IsUser ? ", `creator`" : "";
        var ownerValueDef = entity.Owner.IsUser ? ", @creator" : "";
        var sql = $"""
INSERT INTO `product` (`id`, `name`, `calories`, `proteins`, `fats`, `carbohydrates`{ownerColumDef}) 
values (@id, @name, @calories, @proteins, @fats, @carbohydrates{ownerValueDef});
""";

        await (await connectionProvider.GetConnection()).ExecuteAsync(
            sql,
            new
            {
                id = entity.Id.Value,
                creator = entity.Owner.Owner?.Value,
                name = entity.Name,
                calories = entity.Macronutrients.Calories,
                proteins = entity.Macronutrients.Proteins,
                fats = entity.Macronutrients.Fats,
                carbohydrates = entity.Macronutrients.Carbohydrates,
            }
        );
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var sql = $"""
SELECT 
    `id` as {nameof(Product.Id)}, 
    `name` as {nameof(Product.Name)}, 
    `calories` as {nameof(Macronutrients.Calories)}, 
    `proteins` as {nameof(Macronutrients.Proteins)}, 
    `fats` as {nameof(Macronutrients.Fats)}, 
    `carbohydrates` as {nameof(Macronutrients.Carbohydrates)}, 
    `creator` as {nameof(Product.Create)}
FROM `product`;
""";

        return await (await connectionProvider.GetConnection()).QueryAsync<
            ProductSnapshot,
            MacronutrientsSnapshot,
            Product
        >(
            sql,
            map: (product, macronutrients) =>
            {
                product.Macronutrients = macronutrients;

                return Product.FromSnapshot(product);
            },
            splitOn: nameof(Macronutrients.Calories)
        );
    }

    public Task<IEnumerable<Product>> GetAllWithoutTracking()
    {
        throw new NotImplementedException();
    }

    public async Task<Product?> GetByIdAsync(ProductId id)
    {
        var sql = $"""
SELECT 
    `id` as {nameof(Product.Id)}, 
    `name` as {nameof(Product.Name)}, 
    `calories` as {nameof(Macronutrients.Calories)}, 
    `proteins` as {nameof(Macronutrients.Proteins)}, 
    `fats` as {nameof(Macronutrients.Fats)}, 
    `carbohydrates` as {nameof(Macronutrients.Carbohydrates)}, 
    `creator` as {nameof(Product.Create)}
FROM `product`
WHERE `id` = @id;
""";

        return (
            await (await connectionProvider.GetConnection()).QueryAsync<
                ProductSnapshot,
                MacronutrientsSnapshot,
                Product
            >(
                sql,
                map: (product, macronutrients) =>
                {
                    product.Macronutrients = macronutrients;

                    return Product.FromSnapshot(product);
                },
                new { id = id.Value }
            )
        ).FirstOrDefault();
    }

    public Task<Product> GetByIdAsyncWithoutTracking(ProductId id)
    {
        throw new NotImplementedException();
    }

    public List<Product> GetFavoriteProductList(UserId user)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task RemoveProductFromFavoriteList(UserId user, ProductId product)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }
}
