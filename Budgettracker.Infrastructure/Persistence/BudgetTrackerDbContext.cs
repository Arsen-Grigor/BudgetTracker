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
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var incomeCategory = new RecordCategory(1, "Income");
        var expenseCategory = new RecordCategory(2, "Expenses");
        
        modelBuilder.Entity<RecordCategory>().HasData(incomeCategory, expenseCategory);

        var subcategories = new[]
        {
            new { Id = 1, CategoryId = 1, Name = "Salary" },
            new { Id = 2, CategoryId = 1, Name = "Freelance" },
            new { Id = 3, CategoryId = 1, Name = "Investments" },
            new { Id = 4, CategoryId = 1, Name = "Other" },
            ///////////
            new { Id = 5, CategoryId = 2, Name = "Food" },
            new { Id = 6, CategoryId = 2, Name = "Transportation" },
            new { Id = 7, CategoryId = 2, Name = "Shopping" },
            new { Id = 8, CategoryId = 2, Name = "Entertainment" },
            new { Id = 9, CategoryId = 2, Name = "Bills" },
            new { Id = 10, CategoryId = 2, Name = "Healthcare" },
            new { Id = 11, CategoryId = 2, Name = "Education" },
            new { Id = 12, CategoryId = 2, Name = "Travel" },
            new { Id = 13, CategoryId = 2, Name = "Other" }
        };

        foreach (var sub in subcategories)
        {
            modelBuilder.Entity<RecordSubcategory>().HasData(new RecordSubcategory(sub.Id, sub.CategoryId, sub.Name));
        }
    }
}