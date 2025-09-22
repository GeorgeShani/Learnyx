using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class LessonProgressConfiguration: IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes
        builder.HasIndex(e => new { e.EnrollmentId, e.LessonId }).IsUnique();
        builder.HasIndex(e => e.IsCompleted);
        
        // Foreign key configurations
        builder.HasOne(p => p.Lesson)
            .WithMany(l => l.StudentProgress)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasOne(p => p.Enrollment)
            .WithMany(e => e.LessonProgress)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}