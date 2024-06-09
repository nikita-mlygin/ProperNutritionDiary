namespace ProperNutritionDiary.Product.Domain.Product.External;

public record ExternalProductRepositorySearchResult(
    List<ExternalProduct> Results,
    IExternalProductRepository.Next Next
);
