using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Subtitle).HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.Property(e => e.Language).HasMaxLength(50).HasDefaultValue("English");
        builder.Property(e => e.Duration).HasMaxLength(50);
        builder.Property(e => e.ThumbnailUrl).HasMaxLength(500);
        builder.Property(e => e.PreviewVideoUrl).HasMaxLength(500);
        
        // Decimal precision for prices
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.OriginalPrice).HasPrecision(18, 2);
        
        // JSON columns for collections
        builder.Property(e => e.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
        builder.Property(e => e.Requirements)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());
                
        builder.Property(e => e.LearningOutcomes)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());

        // Complex type for CourseSettings
        builder.OwnsOne(c => c.Settings, settings =>
        {
            settings.Property(s => s.EnableQA).HasDefaultValue(false);
            settings.Property(s => s.EnableReviews).HasDefaultValue(true);
            settings.Property(s => s.EnableDiscussions).HasDefaultValue(false);
            settings.Property(s => s.AutoApprove).HasDefaultValue(true);
            settings.Property(s => s.IssueCertificates).HasDefaultValue(true);
            settings.Property(s => s.SendCompletionEmails).HasDefaultValue(false);
        });
        
        // Indexes
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.Level);
        builder.HasIndex(e => e.TeacherId);
        builder.HasIndex(e => new { e.Status, e.Category });
        
        // Foreign key configurations
        builder.HasOne(c => c.Teacher)
            .WithMany(u => u.CreatedCourses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(c => c.Sections)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(c => c.Reviews)
            .WithOne(r => r.Course)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(c => c.Assignments)
            .WithOne(a => a.Course)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}