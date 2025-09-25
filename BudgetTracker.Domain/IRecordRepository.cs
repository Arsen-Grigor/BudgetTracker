using BudgetTracker.Domain.Entities.Budgets;
using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain;

public interface IRecordRepository
{
    Task AddAsync(Record record, CancellationToken cancellationToken = default);
    Task<Record?> GetByIdAsync(RecordId id, string userId, CancellationToken cancellationToken = default);
    Task<List<Record>> GetByUserAndMonthAsync(string userId, YearMonth month, CancellationToken cancellationToken = default);
    Task<List<Record>> GetBySubcategoryAndMonthAsync(string userId, int subcategoryId, YearMonth month, CancellationToken cancellationToken = default);
    Task<Money> GetTotalSpentForSubcategoryInMonthAsync(string userId, int subcategoryId, YearMonth month, string currency = "USD", CancellationToken cancellationToken = default);
    Task UpdateAsync(Record record, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecordId id, string userId, CancellationToken cancellationToken = default);
}