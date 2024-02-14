namespace ProperNutritionDiary.Test.Product.Test.Persistence;

using DomainDesignLib.Persistence.Repository.Hooks;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence;
using Serilog;
using Xunit.Abstractions;

[Collection("main")]
public class ProductSummaryRepositoryTest
{
    private readonly IProductSummaryRepository productSummaryRepository;
    private readonly IProductRepository productRepository;

    public ServiceProvider ServiceProvider { get; set; }

    public ProductSummaryRepositoryTest(PersistenceContext context, ITestOutputHelper output)
    {
        context.InjectLogging(output);
        this.ServiceProvider = context.ServiceProvider;

        this.productSummaryRepository =
            ServiceProvider.GetService<IProductSummaryRepository>() ?? throw new Exception();
        this.productRepository =
            ServiceProvider.GetService<IProductRepository>() ?? throw new Exception();
    }

    [Fact]
    public async Task AddView_MustExec()
    {
        var user = new User(new UserId(Guid.NewGuid()), UserRole.PlainUser);
        var product = Product
            .Create(
                new ProductId(Guid.NewGuid()),
                "productName",
                Macronutrients.Create(10, 10, 10, 1).Value,
                user,
                DateTime.UtcNow
            )
            .Value;

        await productRepository.CreateAsync(product);

        await productSummaryRepository.AddView(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddView(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddView(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddView(user.Id, product.Id, DateTime.UtcNow);

        var productSummary = await productSummaryRepository.GetById(product.Id);

        productSummary.Should().NotBeNull();
        productSummary!.ViewCount.Should().Be(4);

        true.Should().Be(true);
    }
}
