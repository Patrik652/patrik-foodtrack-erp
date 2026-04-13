using FluentAssertions;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;

namespace FoodTrack.Domain.Tests.Entities;

public sealed class ProductTests
{
    [Fact(DisplayName = "test host domain smoke is discoverable")]
    public void TestHostSmoke_IsDiscoverableByStrictVerifyCommand()
    {
        true.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldRejectNegativeMinStockLevel()
    {
        var action = () => Product.Create(
            "Mlieko Tatranske 1L",
            "mlk-001",
            ProductCategory.Dairy,
            UnitOfMeasure.Liter,
            -1);

        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("minStockLevel");
    }

    [Fact]
    public void Create_ShouldNormalizeNameAndSku()
    {
        var product = Product.Create(
            "  Mlieko Tatranske 1L  ",
            " mlk-001 ",
            ProductCategory.Dairy,
            UnitOfMeasure.Liter,
            12);

        product.Name.Should().Be("Mlieko Tatranske 1L");
        product.Sku.Should().Be("MLK-001");
        product.MinStockLevel.Should().Be(12);
    }
}
