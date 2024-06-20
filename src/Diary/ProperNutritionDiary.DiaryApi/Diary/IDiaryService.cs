using System;
using System.Threading.Tasks;
using LanguageExt;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryApi.Product.Identity.Entity;
using ProperNutritionDiary.DiaryContracts;

namespace ProperNutritionDiary.DiaryApi.Diary;

public interface IDiaryService
{
    /// <summary>
    /// Creates a new diary for a user on a specific date.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="date">The date for which the diary is to be created.</param>
    /// <returns>
    /// A Try containing the unique identifier of the newly created diary if successful,
    /// or an error if the diary already exists for the user on the specified date.
    /// </returns>
    Aff<Guid> CreateDiaryAsync(Guid userId, DateTime date);

    /// <summary>
    /// Updates the date of an existing diary.
    /// </summary>
    /// <param name="diaryId">The unique identifier of the diary to be updated.</param>
    /// <param name="date">The new date to set for the diary.</param>
    /// <returns>A Try containing the unique identifier of the updated diary if successful, or an error if the diary is not found.</returns>
    Aff<Guid> UpdateDiaryAsync(Guid diaryId, DateTime date);

    Aff<Unit> UpdateDiaryEntryAsync(
        Guid diaryId,
        decimal newWeight,
        ConsumptionType consumptionType
    );

    /// <summary>
    /// Retrieves a diary by user ID and date.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="date">The date of the diary to retrieve.</param>
    /// <returns>A Try containing the diary if found, or an error if the diary is not found.</returns>
    Aff<Diary> GetDiaryByDateAsync(Guid userId, DateTime date);

    /// <summary>
    /// Adds an entry to an existing diary.
    /// </summary>
    /// <param name="diaryId">The unique identifier of the diary to add the entry to.</param>
    /// <param name="productIdentity">The product identity associated with the diary entry.</param>
    /// <param name="productName">The name of the product.</param>
    /// <param name="macronutrients">The macronutrients information of the product.</param>
    /// <param name="weight">The weight of the product.</param>
    /// <param name="consumptionTime">The time the product was consumed.</param>
    /// <returns>A Try containing the unique identifier of the updated diary if successful, or an error if the diary is not found.</returns>
    Aff<Guid> AddDiaryEntryAsync(
        Guid diaryId,
        ProductIdentity productIdentity,
        decimal weight,
        DateTime consumptionTime
    );

    /// <summary>
    /// Adds an entry to a diary by date. If the diary does not exist, it is created.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="date">The date of the diary entry.</param>
    /// <param name="productIdentity">The product identity associated with the diary entry.</param>
    /// <param name="weight">The weight of the product.</param>
    /// <param name="consumptionTime">The time the product was consumed.</param>
    /// <returns>A Try containing the unique identifier of the diary entry if successful, or an error if the operation fails.</returns>
    Aff<Guid> AddDiaryEntryByDateAsync(
        Guid userId,
        DateTime date,
        ProductIdentity productIdentity,
        decimal weight,
        DateTime consumptionTime,
        ConsumptionType consumptionType
    );

    /// <summary>
    /// Retrieves a diary by its unique identifier.
    /// </summary>
    /// <param name="diaryId">The unique identifier of the diary to retrieve.</param>
    /// <returns>A Try containing the diary if found, or an error if the diary is not found.</returns>
    Aff<Diary> GetDiaryAsync(Guid diaryId);

    /// <summary>
    /// Deletes a diary by its unique identifier.
    /// </summary>
    /// <param name="diaryId">The unique identifier of the diary to delete.</param>
    /// <returns>A Try indicating the success or failure of the operation.</returns>
    Aff<Unit> DeleteDiaryAsync(Guid diaryId);

    /// <summary>
    /// Deletes a diary entry by its unique identifier and the diary's unique identifier.
    /// </summary>
    /// <param name="diaryId">The unique identifier of the diary containing the entry to delete.</param>
    /// <param name="diaryEntryId">The unique identifier of the diary entry to delete.</param>
    /// <returns>A Try indicating the success or failure of the operation.</returns>
    Aff<Unit> DeleteDiaryEntryAsync(Guid diaryEntryId);
}
