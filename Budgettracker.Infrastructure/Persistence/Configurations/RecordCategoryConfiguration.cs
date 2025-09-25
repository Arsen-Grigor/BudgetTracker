using BudgetTracker.Domain.Entities.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class RecordCategoryConfiguration : IEntityTypeConfiguration<RecordCategory>
{
    public void Configure(EntityTypeBuilder<RecordCategory> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Ignore(c => c.Subcategories);
    }
}