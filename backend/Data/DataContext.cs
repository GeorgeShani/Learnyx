using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageContent> MessageContents { get; set; }
    public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
    public DbSet<AssistantConversationContext> AssistantConversationContexts { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        // Conversation entity configuration
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LastActivityAt);
            entity.HasIndex(e => new { e.User1Id, e.User2Id })
                .IsUnique()
                .HasFilter("[Type] = 0"); // Unique constraint for user-to-user conversations
            entity.HasIndex(e => e.User1Id)
                .HasFilter("[Type] = 1"); // Index for user-to-assistant conversations
            
            // Foreign key configurations
            entity.HasOne(e => e.User1)
                .WithMany()
                .HasForeignKey(e => e.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User2)
                .WithMany()
                .HasForeignKey(e => e.User2Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Message entity configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextContent).HasMaxLength(4000);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.ConversationId, e.CreatedAt });
            entity.HasIndex(e => e.SenderId);

            // Foreign key configurations
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReplyToMessage)
                .WithMany(m => m.Replies)
                .HasForeignKey(e => e.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MessageContent entity configuration
        modelBuilder.Entity<MessageContent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextContent).HasMaxLength(4000);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.HasIndex(e => new { e.MessageId, e.Order });

            // Foreign key configurations
            entity.HasOne(e => e.Message)
                .WithMany(m => m.Contents)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MessageReadStatus entity configuration
        modelBuilder.Entity<MessageReadStatus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.MessageId, e.UserId }).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.Status });

            // Foreign key configurations
            entity.HasOne(e => e.Message)
                .WithMany(m => m.ReadStatuses)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AssistantConversationContext entity configuration
        modelBuilder.Entity<AssistantConversationContext>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SystemPrompt).HasMaxLength(2000);
            entity.HasIndex(e => e.ConversationId).IsUnique();
            entity.HasIndex(e => e.LastInteractionAt);

            // Foreign key configurations
            entity.HasOne(e => e.Conversation)
                .WithOne()
                .HasForeignKey<AssistantConversationContext>(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
