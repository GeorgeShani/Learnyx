using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class CourseEnrollmentConfiguration: IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.AmountPaid).HasPrecision(18, 2);
        builder.Property(e => e.Progress).HasDefaultValue(0);
        
        // Indexes
        builder.HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();
        builder.HasIndex(e => e.EnrollmentDate);
        builder.HasIndex(e => e.IsCompleted);
        
        // Foreign key configurations
        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Student)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.LessonProgress)
            .WithOne(p => p.Enrollment)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}