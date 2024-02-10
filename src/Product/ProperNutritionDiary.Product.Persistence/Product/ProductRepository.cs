namespace ProperNutritionDiary.Product.Persistence.Product;

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DomainDesignLib.Persistence.Repository;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Product.Extensions;
using ProperNutritionDiary.Product.Persistence.Product.Favorite;

internal class ProductRepository(IConnectionProvider connectionProvider) : IProductRepository
{
    private const string table = "product";
    private const string favoriteTable = "favorite_product";
    private const string userId = "user_id";
    private const string favoriteProductRef = "product_id";
    private const string addedToFavoriteAt = "added_at";
    private const string id = "id";
    private const string name = "name";
    private const string calories = "calories";
    private const string proteins = "proteins";
    private const string fats = "fats";
    private const string carbohydrates = "carbohydrates";
    private const string owner = "owner";
    private const string createdAt = "created_at";
    private const string updatedAt = "updated_at";

    private readonly IConnectionProvider connectionProvider = connectionProvider;

    public async Task AddProductToFavoriteList(UserId user, ProductId product, DateTime addedAt)
    {
        var snapshot = new FavoriteProductSnapshot(user.Value, product.Value, addedAt);

        var sql = $"""
INSERT INTO `{favoriteTable}` (`{userId}`, `{favoriteProductRef}`, `{addedToFavoriteAt}`) 
VALUES (
    @{nameof(FavoriteProductSnapshot.UserId)}, 
    @{nameof(FavoriteProductSnapshot.ProductId)}, 
    @{nameof(FavoriteProductSnapshot.AddedAt)});
""";

        await (await this.connectionProvider.Get()).ExecuteAsync(sql, snapshot);
    }

    public async Task CreateAsync(Product entity)
    {
        var ownerColumDef = entity.Owner.IsUser ? $",\n\t`{owner}`" : "";
        var ownerValueDef = entity.Owner.IsUser ? $",\n\t@{nameof(ProductSnapshot.Owner)}" : "";

        var sql = $"""
INSERT INTO `{table}` (
    `{id}`, 
    `{name}`, 
    `{calories}`, 
    `{proteins}`, 
    `{fats}`, 
    `{carbohydrates}`,
    `{createdAt}`{ownerColumDef}) 
VALUES (
    @{nameof(ProductSnapshot.Id)}, 
    @{nameof(ProductSnapshot.Name)}, 
    @{nameof(MacronutrientsSnapshot.Calories)}, 
    @{nameof(MacronutrientsSnapshot.Proteins)}, 
    @{nameof(MacronutrientsSnapshot.Fats)},
    @{nameof(MacronutrientsSnapshot.Carbohydrates)},
    @{nameof(ProductSnapshot.CreatedAt)}{ownerValueDef}
);
""";

        var productSnapshot = entity.ToSnapshot();

        await (await connectionProvider.Get()).ExecuteAsync(sql, productSnapshot.GetParam());
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var sql = $"""
SELECT 
    `{id}` as {nameof(ProductSnapshot.Id)}, 
    `{name}` as {nameof(ProductSnapshot.Name)}, 
    `{owner}` as {nameof(ProductSnapshot.Owner)},
    `{createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{table}`;
""";

        return await (await connectionProvider.Get()).QueryAsync<
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
            splitOn: nameof(MacronutrientsSnapshot.Calories)
        );
    }

    public async Task<Product?> GetByIdAsync(ProductId id)
    {
        var sql = $"""
SELECT 
    `{ProductRepository.id}` as {nameof(ProductSnapshot.Id)}, 
    `{name}` as {nameof(ProductSnapshot.Name)}, 
    `{owner}` as {nameof(ProductSnapshot.Owner)},
    `{createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{table}`
WHERE `{ProductRepository.id}` = @{nameof(ProductSnapshot.Id)};
""";

        return (
            await (await connectionProvider.Get()).QueryAsync<
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
                new { id = id.Value },
                splitOn: nameof(MacronutrientsSnapshot.Calories)
            )
        ).FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> GetFavoriteProductListAsync(UserId user)
    {
        var sql = $"""
SELECT 
    `{id}` as {nameof(ProductSnapshot.Id)}, 
    `{name}` as {nameof(ProductSnapshot.Name)}, 
    `{owner}` as {nameof(ProductSnapshot.Owner)},
    `{createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{table}`
JOIN {favoriteTable} ON {id} = {favoriteProductRef}
WHERE {userId} = @{nameof(FavoriteProductSnapshot.UserId)};
""";

        Dictionary<string, object> param = [];

        param.Add(nameof(FavoriteProductSnapshot.UserId), user.Value);

        return await (await connectionProvider.Get()).QueryAsync<
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
            param,
            splitOn: nameof(MacronutrientsSnapshot.Calories)
        );
    }

    public async Task RemoveAsync(Product entity)
    {
        var productSnapshot = entity.ToSnapshot();

        var sql = $"""
DELETE FROM `{table}` where `{id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, new { id = productSnapshot.Id, });
    }

    public async Task RemoveProductFromFavoriteList(UserId user, ProductId product)
    {
        var snapshot = new FavoriteProductSnapshot(user.Value, product.Value, default);

        var sql = $"""
DELETE FROM {favoriteTable} 
WHERE 
    {userId} = @{nameof(FavoriteProductSnapshot.UserId)} 
    and {favoriteProductRef} = @{nameof(FavoriteProductSnapshot.ProductId)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, snapshot);
    }

    public async Task UpdateAsync(Product entity)
    {
        var ownerSetDef = entity.Owner.IsSystem
            ? ""
            : $",\n\t`{owner}` = @{nameof(ProductSnapshot.Owner)}";
        var productSnapshot = entity.ToSnapshot();

        var sql = $"""
UPDATE `{table}`
SET 
    `{name}` = @{nameof(ProductSnapshot.Name)},
    `{calories}` = @{nameof(MacronutrientsSnapshot.Calories)},
    `{proteins}` = @{nameof(MacronutrientsSnapshot.Proteins)},
    `{fats}` = @{nameof(MacronutrientsSnapshot.Fats)},
    `{carbohydrates}` = @{nameof(MacronutrientsSnapshot.Carbohydrates)}{ownerSetDef},
    `{updatedAt}` = @{nameof(ProductSnapshot.UpdatedAt)}
    where `{id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, productSnapshot.GetParam());
    }
}
