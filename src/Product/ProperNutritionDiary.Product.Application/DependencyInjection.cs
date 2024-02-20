using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ProperNutritionDiary.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}
