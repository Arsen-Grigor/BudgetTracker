using BudgetTracker.Domain.Entities.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class RecordSubcategoryConfiguration : IEntityTypeConfiguration<RecordSubcategory>
{
    public void Configure(EntityTypeBuilder<RecordSubcategory> builder)
    {
        builder.ToTable("Subcategories");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<RecordCategory>()
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.CategoryId);
    }
}