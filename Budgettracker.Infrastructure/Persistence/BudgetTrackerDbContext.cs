using BudgetTracker.Domain.Entities.Budgets;
using BudgetTracker.Domain.Entities.Records;
using BudgetTracker.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Infrastructure.Persistence;

public class BudgetTrackerDbContext : DbContext
{
    public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Record> Records { get; set; }
    public DbSet<MonthCategoryBudget> Budgets { get; set; }
    public DbSet<RecordCategory> Categories { get; set; }
    public DbSet<RecordSubcategory> Subcategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RecordConfiguration());
        modelBuilder.ApplyConfiguration(new MonthCategoryBudgetConfiguration());
        modelBuilder.ApplyConfiguration(new RecordCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new RecordSubcategoryConfiguration());
    }
}