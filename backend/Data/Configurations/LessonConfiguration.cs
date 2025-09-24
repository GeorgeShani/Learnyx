using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class LessonConfiguration: IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Duration).HasMaxLength(20);
        builder.Property(e => e.Content).HasMaxLength(2000);
        
        // JSON column for resources
        builder.Property(e => e.Resources)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        
        // Indexes
        builder.HasIndex(e => new { e.SectionId, e.Order });
        builder.HasIndex(e => e.Type);
        
        // Foreign key configurations
        builder.HasOne(l => l.Section)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(l => l.StudentProgress)
            .WithOne(p => p.Lesson)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(l => l.Assignments)
            .WithOne(a => a.Lesson)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}