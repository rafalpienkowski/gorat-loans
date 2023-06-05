using GoratLoans.Exceptions;

namespace GoratLoans.Inventory;

public class ProductType
{
    public ProductTypeId Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private ProductType(ProductTypeId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static ProductType CreateNew(string name, string description)
    {
        var newProductTypeId = ProductTypeId.NewProductTypeId();

        if (string.IsNullOrEmpty(name))
        {
            throw new GoratLoansException("Name must be not empty");
        }

        if (name.Length < 3)
        {
            throw new GoratLoansException("Name must have more than 3 characters");
        }

        if (name.Length > 40)
        {
            throw new GoratLoansException("Name must have less than 40 characters");
        }

        if (!string.IsNullOrEmpty(description) && description.Length > 200)
        {
            throw new GoratLoansException("Description must have less than 200 characters");
        }

        return new ProductType(newProductTypeId, name, description);
    }
}