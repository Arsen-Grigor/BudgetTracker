using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Commands;
using BudgetTracker.Application.Exceptions;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Queries;
using BudgetTracker.Domain;
using BudgetTracker.Domain.Entities.Budgets;
using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.Services;

public class BudgetTrackerService : IBudgetTrackerService
{
    private readonly IRecordRepository _recordRepository;
    private readonly IMonthCategoryBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;

    public BudgetTrackerService(IRecordRepository recordRepository, 
        IMonthCategoryBudgetRepository budgetRepository, ICategoryRepository categoryRepository)
    {
        _recordRepository = recordRepository;
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
    }
    
    private async Task UpdateBudgetInResponseToUpdateRecordAsync(string userId, int subcategoryId, YearMonth month, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetBySubcategoryAndMonthAsync(userId, subcategoryId, month, cancellationToken);
        if (budget != null)
        {
            var totalSpent = await _recordRepository.GetTotalSpentForSubcategoryInMonthAsync(
                userId, subcategoryId, month, budget.ActualSpent.Currency, cancellationToken);
            budget.AdjustActualSpent(totalSpent);
            await _budgetRepository.UpdateAsync(budget, cancellationToken);
        }
    }

    public async Task<List<RecordDto>> GetRecordsAsync(GetRecordsQuery query, CancellationToken cancellationToken = default)
    {
        var month = new YearMonth(query.Year, query.Month);
        var records = query.SubcategoryId.HasValue
            ? await _recordRepository.GetBySubcategoryAndMonthAsync(query.UserId, query.SubcategoryId.Value, month, cancellationToken)
            : await _recordRepository.GetByUserAndMonthAsync(query.UserId, month, cancellationToken);

        return records.Select(r => new RecordDto(r.Id.Value.ToString(), r.Amount.Amount, r.Amount.Currency,
            r.Category.Name, r.Subcategory.Name, r.Time, r.Description.Text)).ToList();
    }

    public async Task<List<BudgetDto>> GetBudgetsAsync(GetBudgetsQuery query, CancellationToken cancellationToken = default)
    {
        var budgets = await _budgetRepository.GetByUserAndMonthAsync(query.UserId, 
            new YearMonth(query.Year, query.Month), cancellationToken);

        return budgets.Select(b => new BudgetDto(b.Id.Value.ToString(), b.SubCategory.Name,
            b.PlannedBudget.Amount, b.ActualSpent.Amount, b.RemainingBudget.Amount,
            b.BudgetUtilizationPercentage, b.IsOverBudget, b.PlannedBudget.Currency)).ToList();
    }

    public async Task<List<RecordCategoryDto>> GetCategoriesAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var cats = await _categoryRepository.GetAllAsync(cancellationToken);
        return cats.Select(b => new RecordCategoryDto(
            b.Id,
            b.Name,
            b.Subcategories.Select(s => new RecordSubCategoryDto(s.Id, s.CategoryId, s.Name)).ToList()
        )).ToList();
    }
    
    public async Task<string> CreateRecordAsync(CreateRecordCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken)
                       ?? throw new NotFoundException("Category not found");
        
        var subcategory = await _categoryRepository.GetSubcategoryByIdAsync(command.SubcategoryId, cancellationToken);
        if (subcategory?.CategoryId != command.CategoryId)
            throw new BadRequestException("Invalid subcategory");

        var record = Record.Create(RecordId.New(), command.UserId, new Money(command.Amount, command.Currency),
            category, subcategory, command.DateTime, command.Description);

        await _recordRepository.AddAsync(record, cancellationToken);
        await UpdateBudgetInResponseToUpdateRecordAsync(command.UserId, command.SubcategoryId, 
            YearMonth.FromDateTime(command.DateTime), cancellationToken);

        return record.Id.Value.ToString();
    }

    public async Task UpdateRecordAsync(UpdateRecordCommand command, CancellationToken cancellationToken = default)
    {
        var record = await _recordRepository.GetByIdAsync(new RecordId(Guid.Parse(command.RecordId)), 
            command.UserId, cancellationToken) ?? throw new NotFoundException("Record not found");

        if (command.Amount.HasValue)
            record.UpdateAmount(new Money(command.Amount.Value, record.Amount.Currency));
        
        if (!string.IsNullOrEmpty(command.Description))
            record.UpdateDescription(command.Description);

        await _recordRepository.UpdateAsync(record, cancellationToken);
        await UpdateBudgetInResponseToUpdateRecordAsync(command.UserId, record.Subcategory.Id, 
            YearMonth.FromDateTime(record.Time), cancellationToken);
    }

    public async Task DeleteRecordAsync(DeleteRecordCommand command, CancellationToken cancellationToken = default)
    {
        var record = await _recordRepository.GetByIdAsync(new RecordId(Guid.Parse(command.RecordId)), 
            command.UserId, cancellationToken) ?? throw new NotFoundException("Record not found");

        await _recordRepository.DeleteAsync(new RecordId(Guid.Parse(command.RecordId)), 
            command.UserId, cancellationToken);
        await UpdateBudgetInResponseToUpdateRecordAsync(command.UserId, record.Subcategory.Id, 
            YearMonth.FromDateTime(record.Time), cancellationToken);
    }

    public async Task<string> CreateBudgetAsync(CreateBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var subcategory = await _categoryRepository.GetSubcategoryByIdAsync(command.SubcategoryId, cancellationToken)
                          ?? throw new NotFoundException("Subcategory not found");

        var month = new YearMonth(command.Year, command.Month);
        var existing = await _budgetRepository.GetBySubcategoryAndMonthAsync(command.UserId, 
            command.SubcategoryId, month, cancellationToken);
        
        if (existing != null)
            throw new BadRequestException("Budget already exists for this month and subcategory");

        var actualSpent = await _recordRepository.GetTotalSpentForSubcategoryInMonthAsync(
            command.UserId, command.SubcategoryId, month, command.Currency, cancellationToken);

        var budget = MonthCategoryBudget.Create(MonthCategoryBudgetId.New(), command.UserId, month, 
            subcategory, new Money(command.PlannedAmount, command.Currency), actualSpent);

        await _budgetRepository.AddAsync(budget, cancellationToken);
        return budget.Id.Value.ToString();
    }

    public async Task UpdateBudgetAsync(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(new MonthCategoryBudgetId(Guid.Parse(request.BudgetId)), 
            request.UserId, cancellationToken) ?? throw new NotFoundException("Budget not found");

        budget.ChangePlannedBudget(new Money(request.PlannedAmount, budget.PlannedBudget.Currency));
        await _budgetRepository.UpdateAsync(budget, cancellationToken);
    }

    public async Task DeleteBudgetAsync(DeleteBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var budgetId = new MonthCategoryBudgetId(Guid.Parse(command.BudgetId));
        var budget = await _budgetRepository.GetByIdAsync(budgetId, command.UserId, cancellationToken)
                     ?? throw new NotFoundException("Budget not found");

        await _budgetRepository.DeleteAsync(budgetId, command.UserId, cancellationToken);
    }
}