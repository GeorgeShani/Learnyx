using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class MessageContentConfiguration : IEntityTypeConfiguration<MessageContent>
{
    public void Configure(EntityTypeBuilder<MessageContent> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TextContent).HasMaxLength(4000);
        builder.Property(e => e.FileName).HasMaxLength(255);
        builder.Property(e => e.MimeType).HasMaxLength(100);
        builder.Property(e => e.FileUrl).HasMaxLength(500);
        builder.Property(e => e.ThumbnailUrl).HasMaxLength(500);
        builder.HasIndex(e => new { e.MessageId, e.Order });

        // Foreign key configurations
        builder.HasOne(e => e.Message)
            .WithMany(m => m.Contents)
            .HasForeignKey(e => e.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}