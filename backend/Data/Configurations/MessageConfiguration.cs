using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TextContent).HasColumnType("nvarchar(max)");;
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => new { e.ConversationId, e.CreatedAt });
        builder.HasIndex(e => e.SenderId);

        // Foreign key configurations
        builder.HasOne(e => e.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ReplyToMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(e => e.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(m => m.Contents)
            .WithOne(c => c.Message)
            .HasForeignKey(c => c.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.ReadStatuses)
            .WithOne(r => r.Message)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}