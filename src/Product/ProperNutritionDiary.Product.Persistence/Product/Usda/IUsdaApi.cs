using System.Text.Json.Serialization;
using Refit;

namespace ProperNutritionDiary.Product.Persistence.Product.Usda;

public interface IUsdaApi
{
    [Get("/food/{id}")]
    public Task<ApiResponse<string>> Get(string id);

    [Get("/foods/search")]
    public Task<ApiResponse<UsdaSearchResult>> Get(
        [Query] string query,
        [Query] int pageNumber,
        [Query] int pageSize = 200
    );
}

/// <summary>
/// <p>
/// SearchResult:
///   properties:
///     foodSearchCriteria:
///       $ref: '#/components/schemas/FoodSearchCriteria'
///     totalHits:
///       description: The total number of foods found matching the search criteria.
///       type: integer
///       example: 1034
///     currentPage:
///       description: The current page of results being returned.
///       type: integer
///     totalPages:
///       description: The total number of pages found matching the search criteria.
///       type: integer
///     foods:
///       description: The list of foods found matching the search criteria. See Food Fields below.
///       type: array
///       items:
///         $ref: '#/components/schemas/SearchResultFood'
///

/// </p>
/// </summary>
public class UsdaSearchResult
{
    //  AbridgedFoodNutrient:
    //   required:
    //     - id
    //     - nutrientNumber
    //     - unit
    //   properties:
    //     number:
    //       type: integer
    //       format: uint
    //       example: 303
    //     name:
    //       type: string
    //       example: "Iron, Fe"
    //     amount:
    //       type: number
    //       format: float
    //       example: 0.53
    //     unitName:
    //       type: string
    //       example: "mg"
    //     derivationCode:
    //       type: string
    //       example: "LCCD"
    //     derivationDescription:
    //       type: string
    //       example: "Calculated from a daily value percentage per serving size measure"
    public class AbridgedFoodNutrient
    {
        public string Number { get; set; } = string.Empty;
        public string NutrientNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }

    /// SearchResultFood:
    ///   type: object
    ///   required:
    ///     - fdcId
    ///     - description
    ///   properties:
    ///     fdcId:
    ///       description: Unique ID of the food.
    ///       type: integer
    ///       example: 45001529
    ///     dataType:
    ///       description: The type of the food data.
    ///       type: string
    ///       example: "Branded"
    ///     description:
    ///       description: The description of the food.
    ///       type: string
    ///       example: "BROCCOLI"
    ///     foodCode:
    ///       description: Any A unique ID identifying the food within FNDDS.
    ///       type: string
    ///     foodNutrients:
    ///       type: array
    ///      items:
    ///         $ref: '#/components/schemas/AbridgedFoodNutrient'
    ///     publicationDate:
    ///       description: Date the item was published to FDC.
    ///       type: string
    ///       example: "4/1/2019"
    ///     scientificName:
    ///       description: The scientific name of the food.
    ///       type: string
    ///     brandOwner:
    ///       description: Brand owner for the food. Only applies to Branded Foods.
    ///       type: string
    ///       example: "Supervalu, Inc."
    ///     gtinUpc:
    ///       description: GTIN or UPC code identifying the food. Only applies to Branded Foods.
    ///       type: string
    ///       example: "041303020937"
    ///     ingredients:
    ///       description: The list of ingredients (as it appears on the product label). Only applies to Branded Foods.
    ///       type: string
    ///     ndbNumber:
    ///       description: Unique number assigned for foundation foods. Only applies to Foundation and SRLegacy Foods.
    ///       type: integer
    ///     additionalDescriptions:
    ///       description: Any additional descriptions of the food.
    ///       type: string
    ///       example: "Coon; sharp cheese; Tillamook; Hoop; Pioneer; New York; Wisconsin; Longhorn"
    ///     allHighlightFields:
    ///       description: allHighlightFields
    ///       type: string
    ///     score:
    ///       description: Relative score indicating how well the food matches the search criteria.
    ///       type: number
    ///       format: float
    public class SearchResultFood
    {
        public int FdcId { get; set; }
        public string DataType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AbridgedFoodNutrient[] FoodNutrients { get; set; } = [];
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public SearchResultFood[] Foods { get; set; } = [];
}

public class BaseUsdaProductItem
{
    public int Id { get; set; }
    public string DataType { get; set; } = string.Empty;
}
