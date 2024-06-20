// namespace ProperNutritionDiary.DiaryApi.Diary;

// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Transactions;
// using CSharpFunctionalExtensions;
// using DomainDesignLib.Abstractions;
// using Microsoft.EntityFrameworkCore;
// using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
// using ProperNutritionDiary.DiaryApi.Db;
// using ProperNutritionDiary.DiaryApi.Product;
// using ProperNutritionDiary.DiaryApi.Product.Identity;
// using ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

// public class DiaryServiceCopy(AppCtx context, ILogger<DiaryService> logger, IProductApi productApi)
//     : IDiaryService
// {
//     private const string DiaryNotFoundErrorMessage = "Diary not found";
//     private readonly AppCtx ctx = context;
//     private readonly IProductApi productApi = productApi;

//     /// <summary>
//     /// Creates a new diary for a user on a specific date.
//     /// </summary>
//     /// <param name="userId">The unique identifier of the user.</param>
//     /// <param name="date">The date for which the diary is to be created.</param>
//     /// <returns>
//     /// A Result containing the unique identifier of the newly created diary if successful,
//     /// or an error message if the diary already exists for the user on the specified date.
//     /// </returns>
//     public Task<Result<Guid>> CreateDiaryAsync(Guid userId, DateTime date)
//     {
//         // Start with a successful result
//         return Result
//             .Success()
//             // Validate the user ID
//             .Tap(() => ValidateUserId(userId))
//             // Ensure that a diary does not already exist for the user on the specified date
//             .Ensure(
//                 async () =>
//                     !await ctx.Diaries.AnyAsync(d =>
//                         d.UserId == userId && d.Date.Date == date.Date
//                     ),
//                 "Diary already exists for this date"
//             )
//             // Create the diary if the conditions are met
//             .Bind(() => CreateDiaryInternalAsync(userId, date));
//     }

//     public Task<Result<Guid>> UpdateDiaryAsync(Guid diaryId, DateTime date)
//     {
//         return GetDiaryById(diaryId)
//             .ToResult(DiaryNotFoundErrorMessage)
//             .Tap(diary => diary.Date = date)
//             .Tap(() => ctx.SaveChangesAsync())
//             .Map(diary => diary.Id);
//     }

//     public Task<Result<Diary>> GetDiaryByDateAsync(Guid userId, DateTime date)
//     {
//         return ctx
//             .Diaries.Include(d => d.DiaryEntries)
//             .ThenInclude(d => d.Macronutrients)
//             .FirstOrDefaultAsync(d => d.UserId == userId && d.Date.Date == date.Date)
//             .AsMaybe()
//             .ToResult(DiaryNotFoundErrorMessage);
//     }

//     public Task<Result<Guid>> AddDiaryEntryAsync(
//         Guid diaryId,
//         ProductIdentity productIdentity,
//         string productName,
//         Macronutrients macronutrients,
//         decimal weight,
//         DateTime consumptionTime
//     )
//     {
//         return GetDiaryById(diaryId)
//             .ToResult(DiaryNotFoundErrorMessage)
//             .Tap(
//                 (diary) =>
//                     AddDiaryEntryInternal(
//                         diary,
//                         productIdentity,
//                         productName,
//                         macronutrients,
//                         weight,
//                         consumptionTime,
//                         ctx
//                     )
//             )
//             .Tap(diary => logger.LogInformation("Dairy received, {Diary}", diary))
//             .Tap(() => ctx.SaveChangesAsync())
//             .Map(diary => diary.Id);
//     }

//     public static Result<GetProductsRequest> FromProductIdentity(ProductIdentity id)
//     {
//         var request = id.Type switch
//         {
//             SourceType.Barcode
//                 => (id as BarcodeProductIdentity)
//                     .AsMaybe()
//                     .Match(
//                         id =>
//                             Maybe.From(
//                                 new ProductIdDto() { Type = SourceType.Barcode, Value = id.Barcode }
//                             ),
//                         () => Maybe<ProductIdDto>.None
//                     ),
//             SourceType.USDA
//                 => (id as UsdaProductIdentity)
//                     .AsMaybe()
//                     .Match(
//                         id =>
//                             Maybe.From(
//                                 new ProductIdDto() { Type = SourceType.USDA, Value = id.Code }
//                             ),
//                         () => Maybe<ProductIdDto>.None
//                     ),
//             SourceType.EdamamRecipe
//                 => (id as EdamamRecipeProductIdentity)
//                     .AsMaybe()
//                     .Match(
//                         id =>
//                             Maybe.From(
//                                 new ProductIdDto()
//                                 {
//                                     Type = SourceType.EdamamRecipe,
//                                     Value = id.Uri
//                                 }
//                             ),
//                         () => Maybe<ProductIdDto>.None
//                     ),
//             SourceType.System
//                 => (id as SystemProductIdentity)
//                     .AsMaybe()
//                     .Match(
//                         id =>
//                             Maybe.From(
//                                 new ProductIdDto()
//                                 {
//                                     Type = SourceType.System,
//                                     Value = id.Id.ToString()
//                                 }
//                             ),
//                         () => Maybe<ProductIdDto>.None
//                     ),
//             _ => Maybe<ProductIdDto>.None
//         };

//         return request
//             .ToResult("Product identity is invalid")
//             .Map(v => new GetProductsRequest() { ProductIds = [v] });
//     }

//     public Task<Result<Guid>> AddDiaryEntryByDateAsync(
//         Guid userId,
//         DateTime date,
//         ProductIdentity productIdentity,
//         decimal weight,
//         DateTime consumptionTime
//     )
//     {
//         return GetOrCreateDiaryByDateAsync(userId, date)
//             .Tap(diary => logger.LogInformation("Diary received, {@Diary}", diary))
//             .Bind(diary =>
//             {
//                 return FromProductIdentity(productIdentity)
//                     .Tap(rq => logger.LogInformation("Created request: {@Rq}", rq))
//                     .Map(productApi.GetProductsByIds)
//                     .Bind(v => v.FirstOrDefault().AsMaybe().ToResult("Product is not received"))
//                     .Tap(v => logger.LogInformation("Product received, {@Product}", v))
//                     .Map(v =>
//                         AddDiaryEntryInternal(
//                             diary,
//                             productIdentity,
//                             v.Name,
//                             v.Macronutrients,
//                             weight,
//                             consumptionTime,
//                             ctx
//                         )
//                     )
//                     .Tap(() => ctx.SaveChangesAsync());
//             });
//     }

//     private Task<Result<Diary>> GetOrCreateDiaryByDateAsync(Guid userId, DateTime date)
//     {
//         return GetDiaryByDateAsync(userId, date)
//             .Match(
//                 diary => Task.FromResult(Result.Success(diary)),
//                 (err) =>
//                     CreateDiaryInternalAsync(userId, date)
//                         .Bind(id => GetDiaryById(id).ToResult("Error while getting new diary"))
//             );
//     }

//     public Task<Result<Diary>> GetDiaryAsync(Guid diaryId)
//     {
//         return GetDiaryById(diaryId).ToResult(DiaryNotFoundErrorMessage);
//     }

//     public Task<Result> DeleteDiaryAsync(Guid diaryId)
//     {
//         return GetDiaryById(diaryId)
//             .ToResult(DiaryNotFoundErrorMessage)
//             .Tap(diary => ctx.Diaries.Remove(diary))
//             .Tap(() => ctx.SaveChangesAsync())
//             .Match(success => Result.Success(), err => Result.Failure(err));
//     }

//     public Task<Result> DeleteDiaryEntryAsync(Guid diaryId, Guid diaryEntryId)
//     {
//         return GetDiaryById(diaryId)
//             .ToResult(DiaryNotFoundErrorMessage)
//             .Ensure(
//                 diary => diary.DiaryEntries.Any(de => de.Id == diaryEntryId),
//                 "Diary entry not found"
//             )
//             .Tap(diary =>
//             {
//                 var diaryEntry = diary.DiaryEntries.First(de => de.Id == diaryEntryId);
//                 diary.DiaryEntries.Remove(diaryEntry);
//             })
//             .Tap(() => ctx.SaveChangesAsync())
//             .Match(success => Result.Success(), err => Result.Failure(err));
//     }

//     private async Task<Maybe<Diary>> GetDiaryById(Guid diaryId)
//     {
//         return await ctx
//             .Diaries.Include(d => d.DiaryEntries)
//             .FirstOrDefaultAsync(d => d.Id == diaryId)
//             .AsMaybe();
//     }

//     private static Result ValidateUserId(Guid userId)
//     {
//         // Example validation logic: Ensure the userId is not empty
//         if (userId == Guid.Empty)
//         {
//             return Result.Failure("User ID cannot be empty.");
//         }

//         return Result.Success();
//     }

//     private async Task<Result<Guid>> CreateDiaryInternalAsync(Guid userId, DateTime date)
//     {
//         var diary = new Diary
//         {
//             Id = Guid.NewGuid(),
//             UserId = userId,
//             Date = date,
//             DiaryEntries = []
//         };

//         await ctx.Diaries.AddAsync(diary);
//         await ctx.SaveChangesAsync();
//         return diary.Id;
//     }

//     private Guid AddDiaryEntryInternal(
//         Diary diary,
//         ProductIdentity productIdentity,
//         string productName,
//         Macronutrients macronutrients,
//         decimal weight,
//         DateTime consumptionTime,
//         AppCtx ctx
//     )
//     {
//         Guid id = Guid.NewGuid();

//         var diaryEntry = new DiaryEntry
//         {
//             Id = id,
//             Diary = diary,
//             DiaryId = diary.Id,
//             IdType = productIdentity.Type,
//             IdValue = ProductIdentityTypeValueFabric(productIdentity),
//             ProductName = productName,
//             Macronutrients = macronutrients,
//             Weight = weight,
//             ConsumptionTime = consumptionTime
//         };

//         logger.LogInformation("Diary entry to add: {@DiaryEntry}", diaryEntry);

//         ctx.DiaryEntries.Add(diaryEntry);

//         return id;
//     }

//     private static string ProductIdentityTypeValueFabric(ProductIdentity productIdentity)
//     {
//         return productIdentity switch
//         {
//             BarcodeProductIdentity barcode => barcode.Barcode,
//             UsdaProductIdentity usda => usda.Code,
//             SystemProductIdentity system => system.Id.ToString(),
//             EdamamRecipeProductIdentity edamam => edamam.Uri,
//             _ => throw new ArgumentException("Invalid product identity type")
//         };
//     }
// }
