using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        TypeAdapterConfig<UserId, Guid>.NewConfig().MapWith(id => id.Value);
        TypeAdapterConfig<ProductId, Guid>.NewConfig().MapWith(id => id.Value);
        TypeAdapterConfig<Macronutrients, Macronutrients>
            .NewConfig()
            .MapWith(macro =>
                Macronutrients
                    .Create(macro.Calories, macro.Proteins, macro.Fats, macro.Carbohydrates)
                    .Value
            );

        TypeAdapterConfig<ProductOwner, Guid?>.NewConfig().MapWith(owner => null);
        TypeAdapterConfig<ProductOwner, ProductOwner>.NewConfig().MapWith(owner => owner);

        services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}
