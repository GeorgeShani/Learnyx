using System.Text.Json;
using learnyx.Models.Entities;
using learnyx.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.DurationInMonths)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PlanStatus.ACTIVE);

        builder.Property(p => p.MaxCourseAccess)
            .IsRequired()
            .HasDefaultValue(-1);

        builder.Property(p => p.IsPopular)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.Color)
            .HasMaxLength(50);

        builder.Property(p => p.Features)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)
            )
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Plans_Status");

        builder.HasIndex(p => p.Price)
            .HasDatabaseName("IX_Plans_Price");
    }
}