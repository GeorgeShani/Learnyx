using learnyx.Models.Entities;
using learnyx.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TextContent).HasMaxLength(4000);
        builder.Property(e => e.Feedback).HasMaxLength(2000);
        builder.Property(e => e.Status).HasDefaultValue(AssignmentStatus.Pending);
        builder.Property(e => e.IsLate).HasDefaultValue(false);
        builder.Property(e => e.DaysLate).HasDefaultValue(0);
        
        // Indexes
        builder.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique(); // One submission per student per assignment
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.SubmittedAt);
        builder.HasIndex(e => e.GradedAt);
        
        // Foreign key configurations
        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete user when submission is deleted
            
        builder.HasOne(s => s.GradedBy)
            .WithMany(u => u.GradedSubmissions)
            .HasForeignKey(s => s.GradedById)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(s => s.Files)
            .WithOne(f => f.Submission)
            .HasForeignKey(f => f.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}