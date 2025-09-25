using BudgetTracker.Domain.Entities.Budgets;

namespace BudgetTracker.Domain;

public interface IMonthCategoryBudgetRepository
{
    Task AddAsync(MonthCategoryBudget budget, CancellationToken cancellationToken = default);
    Task<MonthCategoryBudget?> GetByIdAsync(MonthCategoryBudgetId id, string userId, CancellationToken cancellationToken = default);
    Task<MonthCategoryBudget?> GetBySubcategoryAndMonthAsync(string userId, int subcategoryId, YearMonth month, CancellationToken cancellationToken = default);
    Task<List<MonthCategoryBudget>> GetByUserAndMonthAsync(string userId, YearMonth month, CancellationToken cancellationToken = default);
    Task UpdateAsync(MonthCategoryBudget budget, CancellationToken cancellationToken = default);
    Task DeleteAsync(MonthCategoryBudgetId id, string userId, CancellationToken cancellationToken = default);
}