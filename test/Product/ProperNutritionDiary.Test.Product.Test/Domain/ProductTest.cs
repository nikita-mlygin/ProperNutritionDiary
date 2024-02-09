namespace ProperNutritionDiary.Test.Product.Test.Domain;

using FluentAssertions;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Create;
using ProperNutritionDiary.Product.Domain.Product.Update;
using ProperNutritionDiary.Product.Domain.User;

public class ProductTest
{
    private const string name = "ProductTestName";
    private readonly ProductId productId = new(Guid.NewGuid());
    private readonly Macronutrients macronutrients = Macronutrients.Create(300, 32, 24, 13).Value;
    private readonly User plainUser = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);
    private readonly User adminUser = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);
    private readonly User anotherPlainUser = new(new UserId(Guid.NewGuid()), UserRole.PlainUser);

    [Fact]
    public void CreateProduct_MustReturnFailed_WhenNameIsNullOrEmpty()
    {
        var product = Product.Create(productId, "", macronutrients, plainUser);

        product.IsFailure.Should().BeTrue();
        product.Error.Should().Be(ProductErrors.NameIsNullOrEmpty);
    }

    [Fact]
    public void CreateProduct_MustReturnSuccessAndRaiseEvent_WhenOk()
    {
        var productResult = Product.Create(productId, name, macronutrients, plainUser);

        productResult.IsFailure.Should().BeFalse();

        var product = productResult.Value;

        var productCreated = product.DomainEvents.OfType<ProductCreated>().First();

        productCreated.Should().NotBeNull();

        productCreated.CreatedProduct.Should().Be(product);
    }

    [Fact]
    public void UpdateProduct_MustReturnFailed_WhenUpdateSystemProductWithNotAdminUpdater()
    {
        var product = Product.Create(productId, name, macronutrients, adminUser).Value;

        var result = product.Update("newName", macronutrients, plainUser);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.UpdateNotAllowedToNoOwner);
    }

    [Fact]
    public void RemoveProduct_MustReturnFailed_WhenRemoveSystemProductWithNotAdminUpdater()
    {
        var product = Product.Create(productId, name, macronutrients, adminUser).Value;

        var result = product.Remove(plainUser);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.RemoveNotAllowedToNoOwner);
    }

    [Fact]
    public void RemoveProduct_MustReturnFailed_WhenRemoveProductWithNoOwner()
    {
        var product = Product.Create(productId, name, macronutrients, plainUser).Value;

        var result = product.Remove(anotherPlainUser);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.RemoveNotAllowedToNoOwner);
    }

    [Fact]
    public void UpdateProduct_MustReturnFailed_WhenUpdateProductWithNoOwner()
    {
        var product = Product.Create(productId, name, macronutrients, plainUser).Value;

        var result = product.Update("newName", macronutrients, anotherPlainUser);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.UpdateNotAllowedToNoOwner);
    }

    [Fact]
    public void UpdateProduct_MustReturnFailed_WhenUpdateProductWithNullOrEmptyName()
    {
        var product = Product.Create(productId, name, macronutrients, plainUser).Value;

        var result = product.Update("", macronutrients, plainUser);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NameIsNullOrEmpty);
    }

    [Fact]
    public void UpdateProduct_MustReturnSuccessAndRaiseEvent_WhenProductUpdateOk()
    {
        var product = Product.Create(productId, name, macronutrients, plainUser).Value;

        var newName = "newName";
        var newMacronutrients = macronutrients + macronutrients;

        var result = product.Update(newName, newMacronutrients, plainUser);

        result.IsSuccess.Should().BeTrue();

        var productUpdated = product.DomainEvents.OfType<ProductUpdated>().First();

        productUpdated.Should().NotBeNull();
        productUpdated.OldName.Should().Be(name);
        productUpdated.OldMacronutrients.Should().Be(macronutrients);
        productUpdated.TargetProduct.Should().Be(product);
    }
}
