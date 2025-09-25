using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Entities.Records;

public sealed class RecordCategory
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public List<RecordSubcategory> Subcategories { get; private set; }

    public RecordCategory(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be null or empty", "INVALID_CATEGORY_NAME");

        Id = id;
        Name = name;
        Subcategories = new List<RecordSubcategory>();
    }

    public void AddSubcategory(RecordSubcategory subcategory)
    {
        ArgumentNullException.ThrowIfNull(subcategory);

        if (subcategory.CategoryId != Id)
            throw new DomainException($"Subcategory belongs to category {subcategory.CategoryId}, not {Id}",
                "SUBCATEGORY_CATEGORY_MISMATCH");

        if (Subcategories.Any(s => s.Id == subcategory.Id))
            throw new DomainException($"Subcategory with ID {subcategory.Id} already exists", "DUPLICATE_SUBCATEGORY");

        Subcategories.Add(subcategory);
    }

    public bool HasSubcategory(int subcategoryId) => Subcategories.Any(s => s.Id == subcategoryId);
}