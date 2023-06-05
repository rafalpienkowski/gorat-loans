using GoratLoans.Exceptions;
using GoratLoans.Inventory;

namespace GoratLoans.Tests.Inventory;

public class ProductTypeShould
{
    private const string ValidProductTypeDescription = "Product type description";
    private const string ValidProductTypeName = "My product type";

    [Fact]
    public void Create_New_Product_Type()
    {
        var productType = ProductType.CreateNew(ValidProductTypeName, ValidProductTypeDescription);

        productType.Id.Should().NotBe(ProductTypeId.Empty);
        productType.Name.Should().Be(ValidProductTypeName);
        productType.Description.Should().Be(ValidProductTypeDescription);
    }

    [Fact]
    public void Create_Product_Type_With_Empty_Description()
    {
        var emptyDescription = string.Empty;
        
        var productType = ProductType.CreateNew(ValidProductTypeName, emptyDescription);

        productType.Id.Should().NotBe(ProductTypeId.Empty);
        productType.Name.Should().Be(ValidProductTypeName);
        productType.Description.Should().Be(emptyDescription);
    }

    [Theory]
    [InlineData(201)]
    [InlineData(2001)]
    public void Reject_Product_Type_With_Too_Long_Description(int descriptionLength)
    {
        var tooLongDescription = string.Concat(Enumerable.Repeat("a", descriptionLength));
        
        Action createAction = () => ProductType.CreateNew(ValidProductTypeName, tooLongDescription);

        createAction.Should().ThrowExactly<GoratLoansException>().WithMessage("Description must have less than 200 characters");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Reject_Product_Type_With_Empty_Name(string emptyName)
    {
        Action createAction = () => ProductType.CreateNew(emptyName, ValidProductTypeDescription);

        createAction.Should().ThrowExactly<GoratLoansException>().WithMessage("Name must be not empty");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    public void Reject_Product_Type_With_Too_Short_Names(string tooShortName)
    {
        Action createAction = () => ProductType.CreateNew(tooShortName, ValidProductTypeDescription);

        createAction.Should().ThrowExactly<GoratLoansException>().WithMessage("Name must have more than 3 characters");
    }

    [Theory]
    [InlineData("01234567890123456789012345678901234567890")]
    [InlineData("01234567890123456789012345678901234567890123456789012345678901234567890123456789")]
    [InlineData("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789")]
    public void Reject_Product_Types_With_Too_Long_Names(string tooLongName)
    {
        Action createAction = () => ProductType.CreateNew(tooLongName, ValidProductTypeDescription);

        createAction.Should().ThrowExactly<GoratLoansException>().WithMessage("Name must have less than 40 characters");
    }
}