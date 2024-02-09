namespace ProperNutritionDiary.Product.Persistence.Product;

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using DomainDesignLib.Persistence.Repository;
using ProperNutritionDiary.BuildingBlocks.PersistencePackages.Connection;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

internal class ProductRepository(IConnectionProvider connectionProvider) : IProductRepository
{
    private const string table = "product";
    private const string id = "id";
    private const string name = "name";
    private const string calories = "calories";
    private const string proteins = "proteins";
    private const string fats = "fats";
    private const string carbohydrates = "carbohydrates";
    private const string owner = "owner";

    private readonly IConnectionProvider connectionProvider = connectionProvider;

    public Task AddProductToFavoriteList(FavoriteProductLineItem favoriteProductListItem)
    {
        throw new NotImplementedException();
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
    `{carbohydrates}`{ownerColumDef}) 
VALUES (
    @{nameof(ProductSnapshot.Id)}, 
    @{nameof(ProductSnapshot.Name)}, 
    @{nameof(MacronutrientsSnapshot.Calories)}, 
    @{nameof(MacronutrientsSnapshot.Proteins)}, 
    @{nameof(MacronutrientsSnapshot.Fats)},
    @{nameof(MacronutrientsSnapshot.Carbohydrates)}{ownerValueDef}
);
""";

        var productSnapshot = entity.ToSnapshot();

        await (await connectionProvider.Get()).ExecuteAsync(
            sql,
            new
            {
                id = productSnapshot.Id,
                owner = productSnapshot.Owner,
                name = productSnapshot.Name,
                calories = productSnapshot.Macronutrients.Calories,
                proteins = productSnapshot.Macronutrients.Proteins,
                fats = productSnapshot.Macronutrients.Fats,
                carbohydrates = productSnapshot.Macronutrients.Carbohydrates,
            }
        );
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var sql = $"""
SELECT 
    `{id}` as {nameof(ProductSnapshot.Id)}, 
    `{name}` as {nameof(ProductSnapshot.Name)}, 
    `{owner}` as {nameof(ProductSnapshot.Owner)},
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

    public Task<IEnumerable<Product>> GetFavoriteProductListAsync(UserId user)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Product entity)
    {
        var productSnapshot = entity.ToSnapshot();

        var sql = $"""
DELETE FROM `{table}` where `{id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(sql, new { id = productSnapshot.Id, });
    }

    public Task RemoveProductFromFavoriteList(UserId user, ProductId product)
    {
        throw new NotImplementedException();
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
    `{carbohydrates}` = @{nameof(MacronutrientsSnapshot.Carbohydrates)}{ownerSetDef}
    where `{id}` = @{nameof(ProductSnapshot.Id)};
""";

        await (await connectionProvider.Get()).ExecuteAsync(
            sql,
            new
            {
                productSnapshot.Id,
                productSnapshot.Name,
                productSnapshot.Owner,
                productSnapshot.Macronutrients.Calories,
                productSnapshot.Macronutrients.Proteins,
                productSnapshot.Macronutrients.Fats,
                productSnapshot.Macronutrients.Carbohydrates
            }
        );
    }
}
