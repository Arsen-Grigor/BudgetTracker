using BudgetTracker.Application.Commands;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Queries;

namespace BudgetTracker.Application.Interfaces;

public interface IBudgetTrackerService
{
    Task<List<RecordDto>> GetRecordsAsync(GetRecordsQuery query, CancellationToken cancellationToken = default);
    Task<List<BudgetDto>> GetBudgetsAsync(GetBudgetsQuery query, CancellationToken cancellationToken = default);
    Task<List<RecordCategoryDto>> GetCategoriesAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default);
    Task<string> CreateRecordAsync(CreateRecordCommand command, CancellationToken cancellationToken = default);
    Task UpdateRecordAsync(UpdateRecordCommand command, CancellationToken cancellationToken = default);
    Task DeleteRecordAsync(DeleteRecordCommand command, CancellationToken cancellationToken = default);
    Task<string> CreateBudgetAsync(CreateBudgetCommand command, CancellationToken cancellationToken = default);
    Task UpdateBudgetAsync(UpdateBudgetCommand command, CancellationToken cancellationToken = default);
    Task DeleteBudgetAsync(DeleteBudgetCommand command, CancellationToken cancellationToken = default);
}