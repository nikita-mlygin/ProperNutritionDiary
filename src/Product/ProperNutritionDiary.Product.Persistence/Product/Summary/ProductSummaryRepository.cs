using System.Data.Common;
using Dapper;
using Dapper.Transaction;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Connection;
using ProperNutritionDiary.Product.Persistence.Product.Summary.Add;
using ProperNutritionDiary.Product.Persistence.Product.Summary.View;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary;

public class ProductSummaryRepository(
    ISqlConnectionProvider sqlConnectionProvider,
    INoSqlConnectionProvider noSqlConnectionProvider
) : IProductSummaryRepository
{
    private readonly ISqlConnectionProvider sqlConnectionProvider = sqlConnectionProvider;
    private readonly INoSqlConnectionProvider noSqlConnectionProvider = noSqlConnectionProvider;
    private string productId = "id";
    private const string productTable = "product";
    private const string viewCount = "view_count";
    private const string addCount = "add_count";

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
UPDATE `{productTable}` 
SET `{addCount}` = `{addCount}` + 1 
WHERE `{productId}` = @{nameof(ProductSnapshot.Id)};
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
UPDATE `{productTable}` 
SET `{viewCount}` = `{viewCount}` + 1 
WHERE `{productId}` = @{nameof(ProductSnapshot.Id)};
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

    public Task<ProductSummary> GetAllPopular()
    {
        throw new NotImplementedException();
    }

    public Task<ProductSummary> GetAllPopular(UserId user)
    {
        throw new NotImplementedException();
    }

    public Task<ProductSummary> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task<bool> HasStatisticsRow(
        Guid userId,
        Guid productId,
        DbConnection connection
    )
    {
        var sql = $"""
SELECT COUNT(*) 
FROM {UserStatisticsTableColumns.table} 
WHERE 
    {UserStatisticsTableColumns.userId} = @{nameof(UserStatisticsSnapshot.UserId)} 
    AND {UserStatisticsTableColumns.productId} = @{nameof(UserStatisticsSnapshot.ProductId)};
""";

        var param = new Dictionary<string, object>()
        {
            { nameof(UserStatisticsSnapshot.ProductId), productId },
            { nameof(UserStatisticsSnapshot.UserId), userId },
        };

        return (await connection.ExecuteScalarAsync<int>(sql, param)) > 0;
    }

    private static async Task AddViewToUserStatistics(
        Guid userId,
        Guid productId,
        DbTransaction transaction
    )
    {
        await (
            await HasStatisticsRow(userId, productId, transaction.Connection!)
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
            await HasStatisticsRow(userId, productId, transaction.Connection!)
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
UPDATE {UserStatisticsTableColumns.table} 
SET {UserStatisticsTableColumns.viewCount} = {UserStatisticsTableColumns.viewCount} + 1 
WHERE
    {UserStatisticsTableColumns.userId} = @{nameof(UserStatisticsSnapshot.UserId)} 
    AND {UserStatisticsTableColumns.productId} = @{nameof(UserStatisticsSnapshot.ProductId)}
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
UPDATE {UserStatisticsTableColumns.table} 
SET {UserStatisticsTableColumns.addCount} = {UserStatisticsTableColumns.addCount} + 1 
WHERE
    {UserStatisticsTableColumns.userId} = @{nameof(UserStatisticsSnapshot.UserId)} 
    AND {UserStatisticsTableColumns.productId} = @{nameof(UserStatisticsSnapshot.ProductId)}
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
INSERT INTO {UserStatisticsTableColumns.table} (
    {UserStatisticsTableColumns.userId},
    {UserStatisticsTableColumns.productId},
    {UserStatisticsTableColumns.viewCount},
    {UserStatisticsTableColumns.addCount}
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
}
