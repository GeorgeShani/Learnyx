using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class AssistantConversationContextConfiguration : IEntityTypeConfiguration<AssistantConversationContext>
{
    public void Configure(EntityTypeBuilder<AssistantConversationContext> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SystemPrompt).HasMaxLength(2000);
        builder.HasIndex(e => e.ConversationId).IsUnique();
        builder.HasIndex(e => e.LastInteractionAt);

        // Foreign key configurations
        builder.HasOne(e => e.Conversation)
            .WithOne()
            .HasForeignKey<AssistantConversationContext>(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
