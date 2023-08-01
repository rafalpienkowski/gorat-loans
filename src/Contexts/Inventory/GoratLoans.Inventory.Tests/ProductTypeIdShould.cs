using GoratLoans.Domain.Exceptions;

namespace GoratLoans.Inventory.Tests;

public class ProductTypeIdShould
{
    [Fact]
    public void Create_New_Product_Type_Id()
    {
        var newProductTypeId = ProductTypeId.NewProductTypeId();

        newProductTypeId.Should().NotBe(ProductTypeId.Empty);
    }

    [Fact]
    public void Reject_Empty_Values()
    {
        var empty = Guid.Empty;

        Action createAction = () => ProductTypeId.From(empty);

        createAction.Should().ThrowExactly<GoratLoansException>().WithMessage("ProductTypeId has to be specified");
    }
}