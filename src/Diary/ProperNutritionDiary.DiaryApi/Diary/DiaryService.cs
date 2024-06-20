namespace ProperNutritionDiary.DiaryApi.Diary;

using System;
using CSharpFunctionalExtensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryApi.Db;
using ProperNutritionDiary.DiaryApi.Diary.Create;
using ProperNutritionDiary.DiaryApi.Product;
using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryApi.Product.Identity.Entity;
using ProperNutritionDiary.DiaryContracts;
using ProperNutritionDiary.DiaryContracts.Add;
using ProperNutritionDiary.DiaryContracts.Product;
using ProperNutritionDiary.DiaryContracts.Remove;
using ProperNutritionDiary.DiaryContracts.Update;

public class DiaryService(
    AppCtx context,
    ILogger<DiaryService> logger,
    IProductApi productApi,
    IPublishEndpoint _publishEndpoint
) : IDiaryService
{
    private readonly AppCtx ctx = context;
    private readonly IProductApi productApi = productApi;
    private readonly IPublishEndpoint publisher = _publishEndpoint;

    public Aff<Guid> CreateDiaryAsync(Guid userId, DateTime date)
    {
        return ValidateUserId(userId)
            .ToAff(err => err.Head)
            .Bind(_ => EnsureDiaryDoesNotExist(userId, date))
            .Bind(_ => CreateDiaryInternalAsync(userId, date));
    }

    private static Validation<Error, Unit> ValidateUserId(Guid userId)
    {
        return userId != Guid.Empty
            ? Success<Error, Unit>(unit)
            : Fail<Error, Unit>(Error.New("User ID cannot be empty."));
    }

    public Aff<Diary> GetDiaryAsync(Guid diaryId)
    {
        return GetDiaryById(diaryId);
    }

    public Aff<Unit> DeleteDiaryAsync(Guid diaryId)
    {
        return GetDiaryById(diaryId)
            .Bind(diary =>
            {
                ctx.Diaries.Remove(diary);
                return SaveChangesAsync();
            });
    }

    public Aff<Unit> DeleteDiaryEntryAsync(Guid diaryEntryId)
    {
        return from diaryEntry in Aff(async () =>
                {
                    var res = await ctx
                        .DiaryEntries.Include(x => x.Diary)
                        .FirstOrDefaultAsync(x => x.Id == diaryEntryId);
                    logger.LogInformation("Diary entry received by id: {@Diary}", res);
                    return res is null ? None : Some(res);
                })
                .Bind(x => x.ToAff(Error.New("Diary entry is not found")))
            from _ in SuccessEff(DeleteDiaryFromCtx(diaryEntry))
                .Do(
                    (de) =>
                    {
                        logger.LogInformation("DiaryEntry success removed, {@DiaryEntry}", de);
                        return unit;
                    }
                )
                .Catch(err =>
                {
                    logger.LogError(err, "Error while delete from ctx");
                    return FailEff<Unit>(err);
                })
            from __ in PublishDeleteDiaryEntryEvent(diaryEntry)
                .Catch(err =>
                {
                    logger.LogError(err, "Error while publishing event");
                    return FailEff<Unit>(err);
                })
            from ___ in SaveChangesAsync()
            select unit;
    }

    private static Option<DiaryEntry> FindDiaryEntry(Diary diary, Guid deId)
    {
        var res = diary.DiaryEntries.Find(x => x.Id == deId);

        if (res is null)
            return None;

        return Some(res);
    }

    private Unit DeleteDiaryFromCtx(DiaryEntry de)
    {
        try
        {
            ctx.DiaryEntries.Remove(de);
            logger.LogInformation("Diary entry removed: {@DiaryEntry}", de);
            return unit;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on removing");
            throw;
        }
    }

    private Aff<Unit> PublishDeleteDiaryEntryEvent(DiaryEntry de)
    {
        logger.LogInformation("Create event for delete diary entry: {@DiaryEntry}", de);

        return Some(de)
            .Map(diary => new DiaryItemRemovedEvent(
                diary.Id,
                DateTime.Now,
                diary.IdType,
                de.IdValue,
                de.Weight,
                de.Macronutrients,
                de.Diary.UserId,
                de.ConsumptionTime,
                de.ConsumptionType
            ))
            .Do(
                (de) =>
                    logger.LogInformation(
                        "Diary entry deleted event: {@DiaryEntryDeletedEvent}",
                        de
                    )
            )
            .ToAff(Error.New("Error while creating diary event"))
            .Bind(de => PublishEvent(de).ToAff());
    }

    private Aff<Unit> EnsureDiaryDoesNotExist(Guid userId, DateTime date)
    {
        return HasDatabaseDiary(userId, date)
            .Bind(result =>
                result
                    ? FailAff<Unit>(Error.New("Diary already exists for this date"))
                    : SuccessAff(unit)
            );
    }

    private Aff<bool> HasDatabaseDiary(Guid userId, DateTime date)
    {
        return ctx.Diaries.AnyAsync(d => d.UserId == userId && d.Date.Date == date.Date).ToAff();
    }

    private Aff<Guid> CreateDiaryInternalAsync(Guid userId, DateTime date)
    {
        return Aff(async () =>
        {
            var diary = new Diary
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                DiaryEntries = new System.Collections.Generic.List<DiaryEntry>()
            };

            await ctx.Diaries.AddAsync(diary);
            await ctx.SaveChangesAsync();
            return diary.Id;
        });
    }

    public Aff<Guid> UpdateDiaryAsync(Guid diaryId, DateTime date)
    {
        return GetDiaryById(diaryId)
            .Bind(diary =>
            {
                diary.Date = date;
                return SaveChangesAsync().Map(_ => diary.Id);
            });
    }

    public Aff<Unit> UpdateDiaryEntryAsync(
        Guid diaryId,
        decimal newWeight,
        ConsumptionType consumptionType
    )
    {
        return from de in GetDiaryEntryById(diaryId)
            from prevValues in Right<
                Error,
                (decimal prevWeight, ConsumptionType prevConsumptionType)
            >(UpdateDiaryEntry(de, newWeight, consumptionType))
                .ToAff()
            from e in Right<Error, DiaryItemUpdatedEvent>(
                    CreateDiaryItemUpdatedEvent(de, prevValues)
                )
                .ToAff()
            from _ in SaveChangesAsync()

            select unit;
    }

    private static (decimal prevWeight, ConsumptionType prevConsumptionType) UpdateDiaryEntry(
        DiaryEntry de,
        decimal newWeight,
        ConsumptionType newConsumptionType
    )
    {
        var res = (de.Weight, de.ConsumptionType);

        de.Weight = newWeight;
        de.ConsumptionType = newConsumptionType;

        return res;
    }

    private static DiaryItemUpdatedEvent CreateDiaryItemUpdatedEvent(
        DiaryEntry de,
        (decimal prevWeight, ConsumptionType prevConsumptionType) prevValues
    )
    {
        (var prevWeight, var prevConsumptionType) = prevValues;
        return new DiaryItemUpdatedEvent(
            de.Id,
            DateTime.Now,
            de.IdType,
            de.IdValue,
            de.Weight,
            prevWeight,
            de.Macronutrients,
            de.Diary.UserId,
            de.ConsumptionTime,
            de.ConsumptionType
        );
    }

    private Aff<Diary> GetDiaryById(Guid diaryId)
    {
        return ctx
            .Diaries.Include(d => d.DiaryEntries)
            .FirstOrDefaultAsync(d => d.Id == diaryId)
            .ToAff()
            .Bind(diary =>
                diary is not null ? SuccessAff(diary) : FailAff<Diary>(Error.New("Diary not found"))
            );
    }

    private Aff<Unit> SaveChangesAsync()
    {
        return Aff(async () =>
        {
            try
            {
                await ctx.SaveChangesAsync();
                return unit;
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, "Error while saving");
                throw;
            }
        });
    }

    public Aff<Guid> AddDiaryEntryByDateAsync(
        Guid userId,
        DateTime date,
        ProductIdentity productIdentity,
        decimal weight,
        DateTime consumptionTime,
        ConsumptionType consumptionType
    )
    {
        return from diaryEntry in GetOrCreateDiaryByDateAsync(userId, date)
                .Bind(diary =>
                {
                    return FromProductIdentity(productIdentity)
                        .Bind(rq => productApi.GetProductsByIds(rq).ToAff())
                        .Map(rq => rq.FirstOrDefault())
                        .Bind(product =>
                            product is not null
                                ? SuccessAff(product)
                                : FailAff<ProductSearchItemDto>(Error.New("Product not found"))
                        )
                        .Map(product =>
                        {
                            var diaryEntry = AddDiaryEntryInternal(
                                diary,
                                productIdentity,
                                product.Name,
                                product.Macronutrients,
                                weight,
                                consumptionTime
                            );
                            return diaryEntry;
                        })
                        .Bind(diaryEntry => SaveChangesAsync().Map(_ => diaryEntry));
                })
            from _ in PublishAddDiaryEvent(diaryEntry)
            select diaryEntry.Id;
    }

    private Aff<Unit> PublishAddDiaryEvent(DiaryEntry addedDiaryEntry)
    {
        return Some(
                new DiaryItemAddedEvent(
                    addedDiaryEntry.Id,
                    DateTime.Now,
                    addedDiaryEntry.IdType,
                    addedDiaryEntry.IdValue,
                    addedDiaryEntry.Weight,
                    addedDiaryEntry.Macronutrients,
                    addedDiaryEntry.Diary.UserId,
                    addedDiaryEntry.ConsumptionTime,
                    addedDiaryEntry.ConsumptionType
                )
            )
            .ToAff()
            .Bind(e => PublishEvent(e).ToAff());
    }

    private async Task<Unit> PublishEvent<T>(T e)
        where T : class
    {
        if (e is null)
            throw new ArgumentNullException(nameof(e), "Event must be not null");
        logger.LogInformation("Event published: {@Event}", e);
        await publisher.Publish<T>(e);
        return unit;
    }

    public Aff<Guid> AddDiaryEntryAsync(
        Guid diaryId,
        ProductIdentity productIdentity,
        decimal weight,
        DateTime consumptionTime
    )
    {
        // TODO
        return Aff(() => Guid.NewGuid().AsValueTask());

        // return GetDiaryById(diaryId)
        //     .Bind(diary =>
        //     {
        //         AddDiaryEntryInternal(diary, productIdentity, weight, consumptionTime);
        //         logger.LogInformation("Diary received, {Diary}", diary);
        //         return SaveChangesAsync().Map(_ => diary.Id);
        //     });
    }

    private Aff<Diary> GetOrCreateDiaryByDateAsync(Guid userId, DateTime date)
    {
        return GetDiaryByDateAsync(userId, date)
            .IfFailAff(CreateDiaryInternalAsync(userId, date).Bind(GetDiaryById));
    }

    public Aff<Diary> GetDiaryByDateAsync(Guid userId, DateTime date)
    {
        return ctx
            .Diaries.Include(d => d.DiaryEntries)
            .ThenInclude(d => d.Macronutrients)
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Date.Date == date.Date)
            .ToAff()
            .Bind(x =>
                x is not null ? SuccessAff(x) : FailAff<Diary>(Error.New("Diary is not found"))
            );
    }

    private DiaryEntry AddDiaryEntryInternal(
        Diary diary,
        ProductIdentity productIdentity,
        string productName,
        Macronutrients macronutrients,
        decimal weight,
        DateTime consumptionTime,
        ConsumptionType consumptionType = ConsumptionType.Other
    )
    {
        var id = Guid.NewGuid();

        var diaryEntry = new DiaryEntry
        {
            Id = id,
            Diary = diary,
            DiaryId = diary.Id,
            IdType = productIdentity.Type,
            IdValue = ProductIdentityTypeValueFabric(productIdentity),
            ProductName = productName,
            Macronutrients = macronutrients,
            Weight = weight,
            ConsumptionTime = consumptionTime,
            ConsumptionType = consumptionType
        };

        logger.LogInformation("Diary entry to add: {@DiaryEntry}", diaryEntry);

        ctx.DiaryEntries.Add(diaryEntry);

        return diaryEntry;
    }

    private Aff<DiaryEntry> GetDiaryEntryById(Guid id)
    {
        return ctx.DiaryEntries.Find(x => x.Id == id).ToAff();
    }

    private static string ProductIdentityTypeValueFabric(ProductIdentity productIdentity)
    {
        return productIdentity switch
        {
            BarcodeProductIdentity barcode => barcode.Barcode,
            UsdaProductIdentity usda => usda.Code,
            SystemProductIdentity system => system.Id.ToString(),
            EdamamRecipeProductIdentity edamam => edamam.Uri,
            _ => throw new ArgumentException("Invalid product identity type")
        };
    }

    public static Eff<GetProductsRequest> FromProductIdentity(ProductIdentity id)
    {
        var request = id.Type switch
        {
            SourceType.Barcode
                => Optional(id as BarcodeProductIdentity)
                    .Map(id => new ProductIdDto { Type = SourceType.Barcode, Value = id.Barcode }),
            SourceType.USDA
                => Optional(id as UsdaProductIdentity)
                    .Map(id => new ProductIdDto { Type = SourceType.USDA, Value = id.Code }),
            SourceType.EdamamRecipe
                => Optional(id as EdamamRecipeProductIdentity)
                    .Map(id => new ProductIdDto { Type = SourceType.EdamamRecipe, Value = id.Uri }),
            SourceType.System
                => Optional(id as SystemProductIdentity)
                    .Map(id => new ProductIdDto
                    {
                        Type = SourceType.System,
                        Value = id.Id.ToString()
                    }),
            _ => Option<ProductIdDto>.None
        };

        return request
            .ToEither(Error.New("Product identity is invalid"))
            .Map(v => new GetProductsRequest { ProductIds = [v] })
            .ToEff();
    }
}
