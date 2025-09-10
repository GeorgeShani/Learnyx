using learnyx.Models.Entities;
using learnyx.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.CourseId)
            .IsRequired();

        builder.Property(e => e.EnrolledAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.CompletedAt)
            .IsRequired(false);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EnrollmentStatus.ACTIVE);

        builder.Property(e => e.ProgressPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);

        builder.Property(e => e.LastAccessedAt)
            .IsRequired(false);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Enrollments_StudentId_CourseId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Enrollments_Status");

        builder.HasIndex(e => e.EnrolledAt)
            .HasDatabaseName("IX_Enrollments_EnrolledAt");
    }
}