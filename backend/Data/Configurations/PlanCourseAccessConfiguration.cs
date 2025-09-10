using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class PlanCourseAccessConfiguration : IEntityTypeConfiguration<PlanCourseAccess>
{
    public void Configure(EntityTypeBuilder<PlanCourseAccess> builder)
    {
        builder.ToTable("PlanCourseAccess");

        builder.HasKey(pca => pca.Id);

        builder.Property(pca => pca.PlanId)
            .IsRequired();

        builder.Property(pca => pca.CourseId)
            .IsRequired();

        builder.Property(pca => pca.AccessStartDate)
            .IsRequired(false);

        builder.Property(pca => pca.AccessEndDate)
            .IsRequired(false);

        builder.Property(pca => pca.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pca => pca.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(pca => pca.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(pca => new { pca.PlanId, pca.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_PlanCourseAccess_PlanId_CourseId");

        builder.HasIndex(pca => pca.IsActive)
            .HasDatabaseName("IX_PlanCourseAccess_IsActive");
    }
}