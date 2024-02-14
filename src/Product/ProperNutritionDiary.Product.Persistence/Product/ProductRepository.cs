namespace ProperNutritionDiary.Product.Persistence.Product;

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DomainDesignLib.Persistence.Repository;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Connection;
using ProperNutritionDiary.Product.Persistence.Product.Extensions;
using ProperNutritionDiary.Product.Persistence.Product.Favorite;
using ProperNutritionDiary.Product.Persistence.Product.TableDefinition;

internal class ProductRepository(ISqlConnectionProvider connectionProvider) : IProductRepository
{
    private readonly IConnectionProvider connectionProvider = connectionProvider;

    public async Task AddProductToFavoriteListAsync(
        UserId user,
        ProductId product,
        DateTime addedAt
    )
    {
        var snapshot = new FavoriteProductSnapshot(user.Value, product.Value, addedAt);

        var sql = $"""
INSERT INTO `{FavoriteTable.table}` (`{FavoriteTable.user}`, `{FavoriteTable.product}`, `{FavoriteTable.addedAt}`) 
VALUES (
    @{nameof(FavoriteProductSnapshot.UserId)}, 
    @{nameof(FavoriteProductSnapshot.ProductId)}, 
    @{nameof(FavoriteProductSnapshot.AddedAt)});
""";

        await (await this.connectionProvider.Get()).ExecuteAsync(sql, snapshot);
    }

    public async Task CreateAsync(Product entity)
    {
        var ownerColumDef = entity.Owner.IsUser ? $",\n\t`{ProductTable.owner}`" : "";
        var ownerValueDef = entity.Owner.IsUser ? $",\n\t@{nameof(ProductSnapshot.Owner)}" : "";

        var sql = $"""
INSERT INTO `{ProductTable.table}` (
    `{ProductTable.id}`, 
    `{ProductTable.name}`, 
    `{ProductTable.calories}`, 
    `{ProductTable.proteins}`, 
    `{ProductTable.fats}`, 
    `{ProductTable.carbohydrates}`,
    `{ProductTable.createdAt}`{ownerColumDef}) 
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
    `{ProductTable.id}` as {nameof(ProductSnapshot.Id)}, 
    `{ProductTable.name}` as {nameof(ProductSnapshot.Name)}, 
    `{ProductTable.owner}` as {nameof(ProductSnapshot.Owner)},
    `{ProductTable.createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{ProductTable.updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{ProductTable.calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{ProductTable.proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{ProductTable.fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{ProductTable.carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{ProductTable.table}`;
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
    `{ProductTable.id}` as {nameof(ProductSnapshot.Id)}, 
    `{ProductTable.name}` as {nameof(ProductSnapshot.Name)}, 
    `{ProductTable.owner}` as {nameof(ProductSnapshot.Owner)},
    `{ProductTable.createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{ProductTable.updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{ProductTable.calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{ProductTable.proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{ProductTable.fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{ProductTable.carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{ProductTable.table}`
WHERE `{ProductTable.id}` = @{nameof(ProductSnapshot.Id)};
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
    `{ProductTable.id}` as {nameof(ProductSnapshot.Id)}, 
    `{ProductTable.name}` as {nameof(ProductSnapshot.Name)}, 
    `{ProductTable.owner}` as {nameof(ProductSnapshot.Owner)},
    `{ProductTable.createdAt}` as {nameof(ProductSnapshot.CreatedAt)},
    `{ProductTable.updatedAt}` as {nameof(ProductSnapshot.UpdatedAt)},
    `{ProductTable.calories}` as {nameof(MacronutrientsSnapshot.Calories)}, 
    `{ProductTable.proteins}` as {nameof(MacronutrientsSnapshot.Proteins)}, 
    `{ProductTable.fats}` as {nameof(MacronutrientsSnapshot.Fats)}, 
    `{ProductTable.carbohydrates}` as {nameof(MacronutrientsSnapshot.Carbohydrates)}
FROM `{ProductTable.table}`
JOIN {FavoriteTable.table} ON {ProductTable.id} = {FavoriteTable.product}
WHERE {FavoriteTable.user} = @{nameof(FavoriteProductSnapshot.UserId)};
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

    public async Task<IEnumerable<UserId>> GetUserWhichFavoriteListContainsProduct(Product product)
    {
        var sql = $"""
SELECT {FavoriteTable.user}
FROM {FavoriteTable.table}
WHERE {FavoriteTable.product} = @{nameof(FavoriteProductSnapshot.ProductId)};
""";

        var param = new DynamicParameters();
        param.Add(nameof(FavoriteProductSnapshot.ProductId), product.Id.Value);

        return (await (await connectionProvider.Get()).QueryAsync<Guid>(sql, param)).Select(
            x => new UserId(x)
        );
    }

    public async Task<bool> IsProductInFavoriteList(UserId user, ProductId product)
    {
        var sql = $"""
SELECT count(*) 
FROM `{FavoriteTable.table}` 
WHERE 
    `{FavoriteTable.product}` = @{nameof(FavoriteProductSnapshot.ProductId)}
    AND `{FavoriteTable.user}` = @{nameof(FavoriteProductSnapshot.UserId)};
""";

        var param = new Dictionary<string, object>()
        {
            { nameof(FavoriteProductSnapshot.ProductId), product.Value },
            { nameof(FavoriteProductSnapshot.UserId), user.Value },
        };

        return (await (await connectionProvider.Get()).ExecuteScalarAsync<int>(sql, param)) == 1;
    }

    public async Task RemoveAsync(Product entity)
    {
        var productSnapshot = entity.ToSnapshot();

        var sql = $"""
DELETE FROM `{ProductTable.table}` where `{ProductTable.id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, new { id = productSnapshot.Id, });
    }

    public async Task RemoveProductFromFavoriteListAsync(UserId user, ProductId product)
    {
        var snapshot = new FavoriteProductSnapshot(user.Value, product.Value, default);

        var sql = $"""
DELETE FROM {FavoriteTable.table} 
WHERE 
    {FavoriteTable.user} = @{nameof(FavoriteProductSnapshot.UserId)} 
    and {FavoriteTable.product} = @{nameof(FavoriteProductSnapshot.ProductId)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, snapshot);
    }

    public async Task UpdateAsync(Product entity)
    {
        var ownerSetDef = entity.Owner.IsSystem
            ? ""
            : $",\n\t`{ProductTable.owner}` = @{nameof(ProductSnapshot.Owner)}";
        var productSnapshot = entity.ToSnapshot();

        var sql = $"""
UPDATE `{ProductTable.table}`
SET 
    `{ProductTable.name}` = @{nameof(ProductSnapshot.Name)},
    `{ProductTable.calories}` = @{nameof(MacronutrientsSnapshot.Calories)},
    `{ProductTable.proteins}` = @{nameof(MacronutrientsSnapshot.Proteins)},
    `{ProductTable.fats}` = @{nameof(MacronutrientsSnapshot.Fats)},
    `{ProductTable.carbohydrates}` = @{nameof(MacronutrientsSnapshot.Carbohydrates)}{ownerSetDef},
    `{ProductTable.updatedAt}` = @{nameof(ProductSnapshot.UpdatedAt)}
    where `{ProductTable.id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, productSnapshot.GetParam());
    }
}
