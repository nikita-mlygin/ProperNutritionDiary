namespace ProperNutritionDiary.Test.Product.Test.Persistence;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
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

    [Fact]
    public async Task AddUse_MustExec()
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

        await productSummaryRepository.AddUse(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddUse(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddUse(user.Id, product.Id, DateTime.UtcNow);
        await productSummaryRepository.AddUse(user.Id, product.Id, DateTime.UtcNow);

        var productSummary = await productSummaryRepository.GetById(product.Id);

        productSummary.Should().NotBeNull();
        productSummary!.UseCount.Should().Be(4);
    }

    [Fact]
    public async Task GetAllPopular_MustExec_WhenAllPopular()
    {
        var res = await productSummaryRepository.GetAllPopular(1);

        res.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllPopular_MustExec_WhenUserPopular()
    {
        var user = new User(new UserId(Guid.NewGuid()), UserRole.PlainUser);

        var products = Enumerable
            .Range(1, 10)
            .Select(x => new
            {
                Product = Product
                    .Create(
                        new ProductId(Guid.NewGuid()),
                        $"product{x}",
                        Macronutrients.Create(x, x, x, x).Value,
                        user,
                        DateTime.UtcNow
                    )
                    .Value,
                Count = x
            })
            .ToList();

        products.ForEach(async x => await productRepository.CreateAsync(x.Product));

        foreach (var x in products)
        {
            for (int i = 0; i < x.Count; i++)
            {
                await productSummaryRepository.AddUse(user.Id, x.Product.Id, DateTime.UtcNow);
                await productSummaryRepository.AddUse(user.Id, x.Product.Id, DateTime.UtcNow);
                await productSummaryRepository.AddView(user.Id, x.Product.Id, DateTime.UtcNow);
            }

            if (x.Count % 2 == 0)
            {
                await productSummaryRepository.AddView(user.Id, x.Product.Id, DateTime.UtcNow);
            }
        }

        var res = await productSummaryRepository.GetAllPopular(user.Id, 1);

        res.Should().NotBeNull();

        // TODO написать дополнения для теста
    }

    [Fact]
    public async Task GetList_MustExec()
    {
        var res = await productSummaryRepository.GetProductList("", null);

        res.Should().NotBeNullOrEmpty();

        var nRes = await productSummaryRepository.GetProductList("", res[2].Id);

        nRes.Should().NotBeNullOrEmpty();

        res[3].Should().Be(nRes[0]);
    }
}
