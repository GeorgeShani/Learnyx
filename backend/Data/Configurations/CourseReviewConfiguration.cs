using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class CourseReviewConfiguration : IEntityTypeConfiguration<CourseReview>
{
    public void Configure(EntityTypeBuilder<CourseReview> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Content).HasMaxLength(1000);
        builder.Property(e => e.Rating).IsRequired();
        
        // Indexes
        builder.HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();
        builder.HasIndex(e => e.Rating);
        
        // Foreign key configurations
        builder.HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(r => r.Student)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}