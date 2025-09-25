using BudgetTracker.Domain.Entities.Budgets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class MonthCategoryBudgetConfiguration : IEntityTypeConfiguration<MonthCategoryBudget>
{
    public void Configure(EntityTypeBuilder<MonthCategoryBudget> builder)
    {
        builder.ToTable("Budgets");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .HasConversion(id => id.Value, value => new MonthCategoryBudgetId(value));

        builder.Property(b => b.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.OwnsOne(b => b.Month, month =>
        {
            month.Property(m => m.Year)
                .HasColumnName("Year");
            month.Property(m => m.Month)
                .HasColumnName("Month");
        });

        builder.OwnsOne(b => b.PlannedBudget, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("PlannedAmount");
            money.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("Currency");
        });

        builder.OwnsOne(b => b.ActualSpent, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("ActualSpent");
            money.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("ActualSpentCurrency");
        });

        builder.HasOne(b => b.SubCategory)
            .WithMany()
            .HasForeignKey("SubcategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex("SubcategoryId");
        }
}