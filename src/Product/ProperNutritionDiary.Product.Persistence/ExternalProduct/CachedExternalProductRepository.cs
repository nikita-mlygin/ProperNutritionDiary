// namespace ProperNutritionDiary.Product.Persistence.Product;

// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using EasyCaching.Core;
// using Microsoft.Extensions.Logging;
// using ProperNutritionDiary.Product.Domain.Product;
// using ProperNutritionDiary.Product.Domain.Product.External;
// using ProperNutritionDiary.UserMenuApi.Product.Entity;

// public class CachedExternalProductRepository(
//     IExternalProductRepository decorated,
//     IEasyCachingProviderFactory cacheProviderFactory,
//     ILogger<CachedExternalProductRepository> logger
// ) : IExternalProductRepository
// {
//     private record ProductResults(List<ProductSnapshot>? Products, int[] Pages);

//     private readonly IExternalProductRepository _decorated = decorated;
//     private readonly IEasyCachingProvider _cacheProvider = cacheProviderFactory.GetCachingProvider(
//         "redis1"
//     );
//     private readonly ILogger<CachedExternalProductRepository> _logger = logger;
//     private const int CacheDurationInDays = 7;

//     public async Task<(List<Product>? products, int[] pageCounts)> Search(
//         string query,
//         int page = 1
//     )
//     {
//         var cacheKey = $"ExternalProductRepository_Search_{query}_{page}";
//         var cacheResult = await _cacheProvider.GetAsync<ProductResults>(cacheKey);

//         if (cacheResult.HasValue)
//         {
//             _logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
//             return (
//                 products: cacheResult.Value.Products?.Select(Product.FromSnapshot).ToList(),
//                 pageCounts: cacheResult.Value.Pages
//             );
//         }

//         _logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);
//         var result = await _decorated.Search(query, page);
//         await _cacheProvider.SetAsync<ProductResults>(
//             cacheKey,
//             new ProductResults(
//                 result.products?.Select(x => x.ToSnapshot()).ToList(),
//                 result.pageCounts
//             ),
//             TimeSpan.FromDays(CacheDurationInDays)
//         );

//         return result;
//     }

//     public Task<Product?> GetFromExternalSource(ExternalSourceIdentity externalSourceIdentity)
//     {
//         return _decorated.GetFromExternalSource(externalSourceIdentity);
//     }
// }
