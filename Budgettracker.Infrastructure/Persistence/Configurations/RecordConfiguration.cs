using BudgetTracker.Domain.Entities.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class RecordConfiguration : IEntityTypeConfiguration<Record>
{
    public void Configure(EntityTypeBuilder<Record> builder)
    {
        builder.ToTable("Records");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new RecordId(value));

        builder.Property(r => r.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.OwnsOne(r => r.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("Amount");
                
            money.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("Currency")
                .HasDefaultValue("USD");
        });

        builder.Property(r => r.Time)
            .HasColumnType("datetime")
            .IsRequired();

        builder.OwnsOne(r => r.Description, desc =>
        {
            desc.Property(d => d.Text)
                .HasMaxLength(500)
                .HasColumnName("Description");
        });

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Subcategory)
            .WithMany()
            .HasForeignKey("SubcategoryId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => new { r.UserId, r.Time });
        builder.HasIndex("SubcategoryId");
    }
}