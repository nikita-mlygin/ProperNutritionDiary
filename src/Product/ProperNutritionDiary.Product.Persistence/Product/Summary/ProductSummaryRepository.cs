using System.Data.Common;
using Dapper;
using Dapper.Transaction;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Connection;
using ProperNutritionDiary.Product.Persistence.Product.Extensions;
using ProperNutritionDiary.Product.Persistence.Product.Summary.Add;
using ProperNutritionDiary.Product.Persistence.Product.Summary.List;
using ProperNutritionDiary.Product.Persistence.Product.Summary.View;
using ProperNutritionDiary.Product.Persistence.Product.TableDefinition;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary;

public class ProductSummaryRepository(
    ISqlConnectionProvider sqlConnectionProvider,
    INoSqlConnectionProvider noSqlConnectionProvider
) : IProductSummaryRepository
{
    private readonly ISqlConnectionProvider sqlConnectionProvider = sqlConnectionProvider;
    private readonly INoSqlConnectionProvider noSqlConnectionProvider = noSqlConnectionProvider;

    private readonly int pageSize = 20;

    public async Task AddUse(UserId user, ProductId product, DateTime addedAt)
    {
        var snapshot = new AddedProductSnapshot()
        {
            UserId = user.Value,
            ProductId = product.Value,
            Id = Guid.NewGuid(),
            AddedAt = addedAt,
        };

        await (await noSqlConnectionProvider.Get()).InsertAsync(snapshot);

        var addTotalViewSql = $"""
UPDATE `{ProductTable.table}`
SET `{ProductTable.addCount}` = `{ProductTable.addCount}` + 1
WHERE `{ProductTable.id}` = @{nameof(ProductSnapshot.Id)};
""";

        using var connection = await sqlConnectionProvider.Get();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        await transaction.ExecuteAsync(
            addTotalViewSql,
            new Dictionary<string, object>() { { nameof(ProductSnapshot.Id), snapshot.ProductId } }
        );

        await AddUserToUserStatistics(user.Value, product.Value, transaction);

        await transaction.CommitAsync();
    }

    public async Task AddView(UserId viewer, ProductId product, DateTime viewedAt)
    {
        var snapshot = new ViewedProductSnapshot()
        {
            UserId = viewer.Value,
            ProductId = product.Value,
            Id = Guid.NewGuid(),
            ViewedAt = viewedAt,
        };

        await (await noSqlConnectionProvider.Get()).InsertAsync(snapshot);

        var sql = $"""
UPDATE `{ProductTable.table}`
SET `{ProductTable.viewCount}` = `{ProductTable.viewCount}` + 1
WHERE `{ProductTable.id}` = @{nameof(ProductSnapshot.Id)};
""";
        using var connection = await sqlConnectionProvider.Get();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        await transaction.ExecuteAsync(
            sql,
            new Dictionary<string, object>() { { nameof(ProductSnapshot.Id), snapshot.ProductId } }
        );

        await AddViewToUserStatistics(viewer.Value, product.Value, transaction);

        await transaction.CommitAsync();
    }

    public async Task<IEnumerable<ProductSummary>> GetAllPopular(int pageNumber)
    {
        var sql = $"""
SELECT
    {ProductTable.id} as {nameof(ProductSummarySnapshot.Id)},
    {ProductTable.name} as {nameof(ProductSummarySnapshot.Name)},
    {ProductTable.owner} as {nameof(ProductSummarySnapshot.Owner)},
    {ProductTable.viewCount} as {nameof(ProductSummarySnapshot.ViewCount)},
    {ProductTable.addCount} as {nameof(ProductSummarySnapshot.AddCount)},
    {ProductTable.calories} as {nameof(ProductSummarySnapshot.Macronutrients.Calories)},
    {ProductTable.proteins} as {nameof(ProductSummarySnapshot.Macronutrients.Proteins)},
    {ProductTable.fats} as {nameof(ProductSummarySnapshot.Macronutrients.Fats)},
    {ProductTable.carbohydrates} as {nameof(ProductSummarySnapshot.Macronutrients.Carbohydrates)}
FROM {ProductTable.table}
ORDER BY
    {ProductTable.addCount} DESC, {ProductTable.viewCount} DESC, {ProductTable.id}
LIMIT
    @{nameof(this.pageSize)}
OFFSET
    @{nameof(pageNumber)};
""";

        var param = new DynamicParameters();

        param.Add(nameof(this.pageSize), pageSize);
        param.Add(nameof(pageNumber), (pageNumber - 1) * pageSize);

        return (
            await (await sqlConnectionProvider.Get()).QueryAsync<
                ProductSummarySnapshot,
                MacronutrientsSnapshot,
                ProductSummary
            >(
                sql,
                (product, macronutrients) =>
                {
                    product.Macronutrients = macronutrients;

                    return ProductSummaryExtensions.FromSnapshot(product);
                },
                param,
                splitOn: nameof(ProductSummarySnapshot.Macronutrients.Calories)
            )
        );
    }

    public async Task<IEnumerable<ProductSummary>> GetAllPopular(UserId user, int pageNumber)
    {
        var sql = $"""
SELECT
    `{ProductTable.table}`.`{ProductTable.id}`
        as `{nameof(ProductSummarySnapshot.Id)}`,
    `{ProductTable.table}`.`{ProductTable.name}`
        as `{nameof(ProductSummarySnapshot.Name)}`,
    `{ProductTable.table}`.`{ProductTable.owner}`
        as `{nameof(ProductSummarySnapshot.Owner)}`,
    `{ProductTable.table}`.`{ProductTable.viewCount}`
        as `{nameof(ProductSummarySnapshot.ViewCount)}`,
    `{ProductTable.table}`.`{ProductTable.addCount}`
        as `{nameof(ProductSummarySnapshot.AddCount)}`,
    `{ProductTable.table}`.`{ProductTable.calories}`
        as `{nameof(ProductSummarySnapshot.Macronutrients.Calories)}`,
    `{ProductTable.table}`.`{ProductTable.proteins}`
        as `{nameof(ProductSummarySnapshot.Macronutrients.Proteins)}`,
    `{ProductTable.table}`.`{ProductTable.fats}`
        as `{nameof(ProductSummarySnapshot.Macronutrients.Fats)}`,
    `{ProductTable.table}`.`{ProductTable.carbohydrates}`
        as `{nameof(ProductSummarySnapshot.Macronutrients.Carbohydrates)}`
FROM `{ProductTable.table}`
LEFT JOIN `{UserStatisticsTable.table}` ON
    `{UserStatisticsTable.table}`.`{UserStatisticsTable.productId}` = `{ProductTable.id}`
    AND `{UserStatisticsTable.table}`.`{UserStatisticsTable.userId}`
        = @{nameof(UserStatisticsSnapshot.UserId)}
ORDER BY
    `{UserStatisticsTable.table}`.`{UserStatisticsTable.addCount}` DESC,
    `{UserStatisticsTable.table}`.`{UserStatisticsTable.viewCount}` DESC,
    `{ProductTable.table}`.`{ProductTable.addCount}` DESC,
    `{ProductTable.table}`.`{ProductTable.viewCount}` DESC,
    `{ProductTable.table}`.`{ProductTable.id}`
LIMIT
    @{nameof(this.pageSize)}
OFFSET
    @{nameof(pageNumber)};
""";

        var param = new DynamicParameters();

        param.Add(nameof(this.pageSize), pageSize);
        param.Add(nameof(pageNumber), (pageNumber - 1) * pageSize);
        param.Add(nameof(UserStatisticsSnapshot.UserId), user.Value);

        return (
            await (await sqlConnectionProvider.Get()).QueryAsync<
                ProductSummarySnapshot,
                MacronutrientsSnapshot,
                ProductSummary
            >(
                sql,
                (product, macronutrients) =>
                {
                    product.Macronutrients = macronutrients;

                    return ProductSummaryExtensions.FromSnapshot(product);
                },
                param,
                splitOn: nameof(ProductSummarySnapshot.Macronutrients.Calories)
            )
        );
    }

    public async Task<ProductSummary?> GetById(ProductId id)
    {
        var sql = $"""
SELECT
    {ProductTable.id} as {nameof(ProductSummarySnapshot.Id)},
    {ProductTable.name} as {nameof(ProductSummarySnapshot.Name)},
    {ProductTable.owner} as {nameof(ProductSummarySnapshot.Owner)},
    {ProductTable.viewCount} as {nameof(ProductSummarySnapshot.ViewCount)},
    {ProductTable.addCount} as {nameof(ProductSummarySnapshot.AddCount)},
    {ProductTable.calories} as {nameof(ProductSummarySnapshot.Macronutrients.Calories)},
    {ProductTable.proteins} as {nameof(ProductSummarySnapshot.Macronutrients.Proteins)},
    {ProductTable.fats} as {nameof(ProductSummarySnapshot.Macronutrients.Fats)},
    {ProductTable.carbohydrates} as {nameof(ProductSummarySnapshot.Macronutrients.Carbohydrates)}
FROM {ProductTable.table}
WHERE
    {ProductTable.id} = @{nameof(ProductSummarySnapshot.Id)}
""";

        return (
            await (await sqlConnectionProvider.Get()).QueryAsync<
                ProductSummarySnapshot,
                MacronutrientsSnapshot,
                ProductSummary
            >(
                sql,
                (product, macronutrients) =>
                {
                    product.Macronutrients = macronutrients;

                    return ProductSummaryExtensions.FromSnapshot(product);
                },
                new Dictionary<string, object>()
                {
                    { nameof(ProductSummarySnapshot.Id), id.Value }
                },
                splitOn: nameof(ProductSummarySnapshot.Macronutrients.Calories)
            )
        ).FirstOrDefault();
    }

    private static async Task<bool> HasStatisticsRow(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        var sql = $"""
SELECT COUNT(*)
FROM {UserStatisticsTable.table}
WHERE
    {UserStatisticsTable.userId} = @{nameof(UserStatisticsSnapshot.UserId)}
    AND {UserStatisticsTable.productId} = @{nameof(UserStatisticsSnapshot.ProductId)};
""";

        var param = new Dictionary<string, object>()
        {
            { nameof(UserStatisticsSnapshot.ProductId), productId },
            { nameof(UserStatisticsSnapshot.UserId), userId },
        };

        return (await transaction.ExecuteScalarAsync<int>(sql, param)) > 0;
    }

    private static async Task AddViewToUserStatistics(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        await (
            await HasStatisticsRow(userId, productId, transaction)
                ? UpdateViewInUserStatistics(userId, productId, transaction)
                : CreateUserStatistics(
                    new UserStatisticsSnapshot()
                    {
                        UserId = userId,
                        ProductId = productId,
                        ViewCount = 1,
                        AddCount = 0
                    },
                    transaction
                )
        );
    }

    private static async Task AddUserToUserStatistics(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        await (
            await HasStatisticsRow(userId, productId, transaction)
                ? UpdateUseInUserStatistics(userId, productId, transaction)
                : CreateUserStatistics(
                    new UserStatisticsSnapshot()
                    {
                        UserId = userId,
                        ProductId = productId,
                        ViewCount = 0,
                        AddCount = 1
                    },
                    transaction
                )
        );
    }

    private static async Task UpdateViewInUserStatistics(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        var sql = $"""
UPDATE {UserStatisticsTable.table}
SET {UserStatisticsTable.viewCount} = {UserStatisticsTable.viewCount} + 1
WHERE
    {UserStatisticsTable.userId} = @{nameof(UserStatisticsSnapshot.UserId)}
    AND {UserStatisticsTable.productId} = @{nameof(UserStatisticsSnapshot.ProductId)}
""";
        var param = new Dictionary<string, object>()
        {
            { nameof(UserStatisticsSnapshot.ProductId), productId },
            { nameof(UserStatisticsSnapshot.UserId), userId },
        };

        await transaction.ExecuteAsync(sql, param);
    }

    private static async Task UpdateUseInUserStatistics(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        var sql = $"""
UPDATE {UserStatisticsTable.table}
SET {UserStatisticsTable.addCount} = {UserStatisticsTable.addCount} + 1
WHERE
    {UserStatisticsTable.userId} = @{nameof(UserStatisticsSnapshot.UserId)}
    AND {UserStatisticsTable.productId} = @{nameof(UserStatisticsSnapshot.ProductId)}
""";
        var param = new Dictionary<string, object>()
        {
            { nameof(UserStatisticsSnapshot.ProductId), productId },
            { nameof(UserStatisticsSnapshot.UserId), userId },
        };

        await transaction.ExecuteAsync(sql, param);
    }

    private static async Task CreateUserStatistics(
        UserStatisticsSnapshot snapshot,
        DbTransaction transaction
    )
    {
        var sql = $"""
INSERT INTO {UserStatisticsTable.table} (
    {UserStatisticsTable.userId},
    {UserStatisticsTable.productId},
    {UserStatisticsTable.viewCount},
    {UserStatisticsTable.addCount}
)
VALUES (
    @{nameof(UserStatisticsSnapshot.UserId)},
    @{nameof(UserStatisticsSnapshot.ProductId)},
    @{nameof(UserStatisticsSnapshot.ViewCount)},
    @{nameof(UserStatisticsSnapshot.AddCount)}
);
""";

        await transaction.ExecuteAsync(sql, snapshot);
    }

    public async Task<List<ProductListSummary>> GetProductList(
        string nameFilter,
        ProductId? lastProduct
    )
    {
        string sql;
        var param = new DynamicParameters();

        if (lastProduct is not null)
        {
            sql = $"""
SELECT
    {ProductTable.id} as {nameof(ProductListSummarySnapshot.Id)},
    {ProductTable.name} as {nameof(ProductListSummarySnapshot.Name)},
    {ProductTable.owner} as {nameof(ProductListSummarySnapshot.Owner)}
FROM {ProductTable.table}
WHERE
    {ProductTable.name} like @{nameof(nameFilter)}
    AND {ProductTable.num} > (
        SELECT {ProductTable.num}
        FROM {ProductTable.table}
        WHERE {ProductTable.id} = @{nameof(ProductSnapshot.Id)})
ORDER BY {ProductTable.name}, {ProductTable.num}
LIMIT @{nameof(pageSize)};
""";

            param.Add(nameof(ProductSnapshot.Id), lastProduct.Value);
        }
        else
        {
            sql = $"""
SELECT
    {ProductTable.id} as {nameof(ProductListSummarySnapshot.Id)},
    {ProductTable.name} as {nameof(ProductListSummarySnapshot.Name)},
    {ProductTable.owner} as {nameof(ProductListSummarySnapshot.Owner)}
FROM {ProductTable.table}
WHERE
    {ProductTable.name} like @{nameof(nameFilter)}
ORDER BY {ProductTable.name}, {ProductTable.num}
LIMIT @{nameof(pageSize)};
""";
        }

        param.Add(nameof(nameFilter), $"%{nameFilter}%");
        param.Add(nameof(pageSize), pageSize);

        return (
            await (await sqlConnectionProvider.Get()).QueryAsync<ProductListSummarySnapshot>(
                sql,
                param
            )
        )
            .Select(s => s.To())
            .ToList();
    }
}
