namespace ProperNutritionDiary.Product.Persistence.Mappings;

using Cassandra.Mapping;
using ProperNutritionDiary.Product.Persistence.Product.Summary.Add;
using ProperNutritionDiary.Product.Persistence.Product.Summary.View;

public class GlobalMappingsDefinition : Mappings
{
    public GlobalMappingsDefinition()
    {
        For<AddedProductSnapshot>()
            .TableName("product_add")
            .PartitionKey(u => u.ProductId, u => u.UserId, u => u.Id)
            .Column(u => u.Id, cl => cl.WithName("id"))
            .Column(u => u.UserId, cl => cl.WithName("user_id"))
            .Column(u => u.ProductId, cl => cl.WithName("product_id"))
            .Column(u => u.AddedAt, cl => cl.WithName("add_at"));

        For<ViewedProductSnapshot>()
            .TableName("product_view")
            .PartitionKey(u => u.ProductId, u => u.UserId, u => u.Id)
            .Column(u => u.Id, cl => cl.WithName("id"))
            .Column(u => u.UserId, cl => cl.WithName("user_id"))
            .Column(u => u.ProductId, cl => cl.WithName("product_id"))
            .Column(u => u.ViewedAt, cl => cl.WithName("view_at"));
    }
}
