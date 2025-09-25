using BudgetTracker.Domain;
using BudgetTracker.Domain.Entities.Budgets;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.Repositories;

public class MonthCategoryBudgetRepository : BaseSecureRepository, IMonthCategoryBudgetRepository
{
    public MonthCategoryBudgetRepository(BudgetTrackerDbContext context, ILogger<MonthCategoryBudgetRepository> logger) 
        : base(context, logger) { }

    public async Task AddAsync(MonthCategoryBudget budget, CancellationToken cancellationToken = default)
    {
        ValidateUserId(budget.UserId);
        await _context.Budgets.AddAsync(budget, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MonthCategoryBudget?> GetByIdAsync(MonthCategoryBudgetId id, string userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        return await _context.Budgets
            .Include(b => b.SubCategory)
            .Where(b => b.Id == id && b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MonthCategoryBudget?> GetBySubcategoryAndMonthAsync(string userId, int subcategoryId, YearMonth month, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        if (subcategoryId <= 0)
            throw new ArgumentException("Invalid subcategory ID", nameof(subcategoryId));
        
        return await _context.Budgets
            .Include(b => b.SubCategory)
            .Where(b => b.UserId == userId && 
                       b.SubCategory.Id == subcategoryId &&
                       b.Month.Year == month.Year &&
                       b.Month.Month == month.Month)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MonthCategoryBudget>> GetByUserAndMonthAsync(string userId, YearMonth month, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        return await _context.Budgets
            .Include(b => b.SubCategory)
            .Where(b => b.UserId == userId &&
                       b.Month.Year == month.Year &&
                       b.Month.Month == month.Month)
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(MonthCategoryBudget budget, CancellationToken cancellationToken = default)
    {
        ValidateUserId(budget.UserId);
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MonthCategoryBudgetId id, string userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        var budget = await _context.Budgets
            .Where(b => b.Id == id && b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
