using BudgetTracker.Domain;
using BudgetTracker.Domain.Entities.Budgets;
using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.Repositories;

public class RecordRepository : BaseSecureRepository, IRecordRepository
{
    public RecordRepository(BudgetTrackerDbContext context, ILogger<RecordRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task AddAsync(Record record, CancellationToken cancellationToken = default)
    {
        ValidateUserId(record.UserId);
        await _context.Records.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Record?> GetByIdAsync(RecordId id, string userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        return await _context.Records
            .Include(r => r.Category)
            .Include(r => r.Subcategory)
            .Where(r => r.Id == id && r.UserId == userId) // Security: User isolation
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Record>> GetByUserAndMonthAsync(string userId, YearMonth month, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        var startDate = month.FirstDayOfMonth;
        var endDate = month.LastDayOfMonth;
        
        return await _context.Records
            .Include(r => r.Category)
            .Include(r => r.Subcategory)
            .Where(r => r.UserId == userId && 
                       r.Time >= startDate && 
                       r.Time <= endDate) // Security: Date range validation
            .OrderByDescending(r => r.Time)
            .Take(1000) // Security: Limit results to prevent DoS
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Record>> GetBySubcategoryAndMonthAsync(string userId, int subcategoryId, YearMonth month, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        if (subcategoryId <= 0)
            throw new ArgumentException("Invalid subcategory ID", nameof(subcategoryId));
            
        var startDate = month.FirstDayOfMonth;
        var endDate = month.LastDayOfMonth;
        
        return await _context.Records
            .Include(r => r.Category)
            .Include(r => r.Subcategory)
            .Where(r => r.UserId == userId && 
                       r.Subcategory.Id == subcategoryId &&
                       r.Time >= startDate && 
                       r.Time <= endDate)
            .OrderByDescending(r => r.Time)
            .Take(500) // Security: Limit results
            .ToListAsync(cancellationToken);
    }

    public async Task<Money> GetTotalSpentForSubcategoryInMonthAsync(string userId, int subcategoryId, YearMonth month, string currency = "USD", CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        if (subcategoryId <= 0)
            throw new ArgumentException("Invalid subcategory ID", nameof(subcategoryId));
            
        var startDate = month.FirstDayOfMonth;
        var endDate = month.LastDayOfMonth;
        
        var total = await _context.Records
            .Where(r => r.UserId == userId && 
                       r.Subcategory.Id == subcategoryId &&
                       r.Time >= startDate && 
                       r.Time <= endDate)
            .SumAsync(r => r.Amount.Amount, cancellationToken);
            
        return new Money(Math.Abs(total), currency); // Ensure positive for expenses
    }

    public async Task UpdateAsync(Record record, CancellationToken cancellationToken = default)
    {
        ValidateUserId(record.UserId);
        _context.Records.Update(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RecordId id, string userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        
        var record = await _context.Records
            .Where(r => r.Id == id && r.UserId == userId) // Security: User isolation
            .FirstOrDefaultAsync(cancellationToken);
            
        if (record != null)
        {
            _context.Records.Remove(record);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}