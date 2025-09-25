using BudgetTracker.Domain.Entities.Records;

namespace BudgetTracker.Application.DTOs;

public sealed class RecordCategoryDto(int id, string name, List<RecordSubCategoryDto> subcategories)
{
    public int Id { get; init; } = id;
    public string Name { get; init; } = name;
    public List<RecordSubCategoryDto> Subcategories { get; init; } = subcategories;
}