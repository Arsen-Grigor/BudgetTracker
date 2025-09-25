using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Entities.Records;

public sealed record RecordSubcategory
{
    public int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Name { get; private set; }
    
    public RecordSubcategory(int id, int categoryId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Subcategory name cannot be null or empty", "INVALID_SUBCATEGORY_NAME");
            
        Id = id;
        CategoryId = categoryId;
        Name = name;
    }
}