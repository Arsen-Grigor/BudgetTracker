using BudgetTracker.Domain;
using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.Repositories;

public class CategoryRepository : BaseSecureRepository, ICategoryRepository
{
    public CategoryRepository(BudgetTrackerDbContext context, ILogger<CategoryRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<RecordCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        if (categoryId <= 0)
            return null;
            
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
            
        if (category != null)
        {
            var subcategories = await _context.Subcategories
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync(cancellationToken);
                
            foreach (var subcategory in subcategories)
            {
                category.AddSubcategory(subcategory);
            }
        }
        
        return category;
    }

    public async Task<List<RecordCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        
        foreach (var category in categories)
        {
            var subcategories = await _context.Subcategories
                .Where(s => s.CategoryId == category.Id)
                .ToListAsync(cancellationToken);
                
            foreach (var subcategory in subcategories)
            {
                category.AddSubcategory(subcategory);
            }
        }
        
        return categories;
    }

    public async Task<RecordSubcategory?> GetSubcategoryByIdAsync(int subcategoryId, CancellationToken cancellationToken = default)
    {
        if (subcategoryId <= 0)
            return null;
            
        return await _context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == subcategoryId, cancellationToken);
    }
}