using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.LastActivityAt);
        builder.HasIndex(e => new { e.User1Id, e.User2Id })
            .IsUnique()
            .HasFilter("[Type] = 0"); // Unique constraint for user-to-user conversations
        builder.HasIndex(e => e.User1Id)
            .HasFilter("[Type] = 1"); // Index for user-to-assistant conversations
        
        // Foreign key configurations
        builder.HasOne(e => e.User1)
            .WithMany()
            .HasForeignKey(e => e.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User2)
            .WithMany()
            .HasForeignKey(e => e.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}