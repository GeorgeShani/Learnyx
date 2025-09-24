using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Instructions).HasMaxLength(4000);
        builder.Property(e => e.MaxPoints).HasDefaultValue(100);
        builder.Property(e => e.AllowLateSubmission).HasDefaultValue(true);
        builder.Property(e => e.LatePenaltyPercentage).HasDefaultValue(10);
        builder.Property(e => e.IsVisible).HasDefaultValue(true);
        
        // Indexes
        builder.HasIndex(e => new { e.CourseId, e.Order });
        builder.HasIndex(e => e.DueDate);
        builder.HasIndex(e => e.IsVisible);
        
        // Foreign key configurations
        builder.HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(a => a.Lesson)
            .WithMany(l => l.Assignments)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasMany(a => a.Submissions)
            .WithOne(s => s.Assignment)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(a => a.Resources)
            .WithOne(r => r.Assignment)
            .HasForeignKey(r => r.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}