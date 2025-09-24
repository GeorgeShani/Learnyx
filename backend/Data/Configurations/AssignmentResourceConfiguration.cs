using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class AssignmentResourceConfiguration : IEntityTypeConfiguration<AssignmentResource>
{
    public void Configure(EntityTypeBuilder<AssignmentResource> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.FilePath).HasMaxLength(500);
        builder.Property(e => e.ExternalUrl).HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsRequired).HasDefaultValue(false);
        
        // Indexes
        builder.HasIndex(e => new { e.AssignmentId, e.Order });
        builder.HasIndex(e => e.Type);
        
        // Foreign key configurations
        builder.HasOne(r => r.Assignment)
            .WithMany(a => a.Resources)
            .HasForeignKey(r => r.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}