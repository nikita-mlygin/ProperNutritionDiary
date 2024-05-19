namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public static class ProductIdentityExtensions
{
    public static ProductIdentityFunctionBuilder<TRes> For<TRes, T>(
        this ExternalSourceIdentity productIdentity,
        Func<T, TRes> func
    )
        where T : ExternalSourceIdentity
    {
        return new ProductIdentityFunctionBuilder<TRes>(
            (identity) =>
            {
                return func((T)identity);
            },
            productIdentity,
            productIdentity.GetType()
        );
    }
}

public class ProductIdentityFunctionBuilder<TRes>
{
    private readonly Func<ExternalSourceIdentity, TRes>? function = null;
    private readonly ExternalSourceIdentity identity;

    public ProductIdentityFunctionBuilder(
        Func<ExternalSourceIdentity, TRes> action,
        ExternalSourceIdentity identity,
        Type type
    )
    {
        this.identity = identity;

        if (identity.GetType().IsAssignableTo(type))
        {
            function = action;
        }
    }

    public ProductIdentityFunctionBuilder(
        ProductIdentityFunctionBuilder<TRes> prev,
        Func<ExternalSourceIdentity, TRes> next,
        Type type
    )
    {
        this.identity = prev.identity;
        this.function = prev.function;

        Console.WriteLine(
            $"\tType: {type.FullName}\n\tCurrentType: {identity.GetType().FullName}\n\tCurrentTypeIsAssignableToType: {identity.GetType().IsAssignableTo(type)}"
        );

        if (identity.GetType().IsAssignableTo(type))
        {
            function = next;
        }
    }

    public TRes? Build()
    {
        if (function is not null)
        {
            return function(identity);
        }

        return default;
    }

    public static implicit operator TRes?(ProductIdentityFunctionBuilder<TRes> builder)
    {
        return builder.Build();
    }
}

public static class ProductIdentityFunctionBuilderExtensions
{
    public static ProductIdentityFunctionBuilder<TRes> For<TRes, T>(
        this ProductIdentityFunctionBuilder<TRes> productIdentityFunctionBuilder,
        Func<T, TRes> func
    )
        where T : ExternalSourceIdentity
    {
        return new ProductIdentityFunctionBuilder<TRes>(
            productIdentityFunctionBuilder,
            (identity) =>
            {
                return func((T)identity);
            },
            typeof(T)
        );
    }
}
