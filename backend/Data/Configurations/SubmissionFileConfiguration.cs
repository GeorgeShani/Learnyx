using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
{
    public void Configure(EntityTypeBuilder<SubmissionFile> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.StoredFileName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        
        // Indexes
        builder.HasIndex(e => new { e.SubmissionId, e.Order });
        builder.HasIndex(e => e.StoredFileName).IsUnique();
        
        // Foreign key configurations
        builder.HasOne(f => f.Submission)
            .WithMany(s => s.Files)
            .HasForeignKey(f => f.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}