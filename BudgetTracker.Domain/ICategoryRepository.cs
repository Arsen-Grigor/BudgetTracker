using BudgetTracker.Domain.Entities.Records;

namespace BudgetTracker.Domain;

public interface ICategoryRepository
{
    Task<RecordCategory?> GetByIdAsync(int categoryId,
        CancellationToken cancellationToken = default);
    Task<List<RecordCategory>> GetAllAsync(
        CancellationToken cancellationToken = default);
    Task<RecordSubcategory?> GetSubcategoryByIdAsync(int subcategoryId,
        CancellationToken cancellationToken = default);
}