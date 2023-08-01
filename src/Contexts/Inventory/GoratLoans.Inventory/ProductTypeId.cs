using GoratLoans.Domain.Exceptions;

namespace GoratLoans.Inventory;

public record ProductTypeId
{
    public Guid Value { get; }
    
    private ProductTypeId(Guid value)
    {
        Value = value;
    }

    public static ProductTypeId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new GoratLoansException("ProductTypeId has to be specified");
        }
        
        return new ProductTypeId(value);
    }
    
    public static ProductTypeId NewProductTypeId() => new(Guid.NewGuid());

    public static ProductTypeId Empty => new(Guid.Empty);
}