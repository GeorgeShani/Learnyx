using learnyx.Data.Configurations;
using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Data;

public class DataContext : DbContext
{
    // Chat/Messaging DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageContent> MessageContents { get; set; }
    public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
    public DbSet<AssistantConversationContext> AssistantConversationContexts { get; set; }
    
    // Course/Learning DbSets
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseSection> CourseSections { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<CourseReview> CourseReviews { get; set; }
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    public DbSet<LessonProgress> LessonProgress { get; set; }
    public DbSet<CourseCategory> CourseCategories { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ConversationConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new MessageContentConfiguration());
        modelBuilder.ApplyConfiguration(new MessageReadStatusConfiguration());
        modelBuilder.ApplyConfiguration(new AssistantConversationContextConfiguration());
        
        // Course-related configurations
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new CourseSectionConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        modelBuilder.ApplyConfiguration(new CourseReviewConfiguration());
        modelBuilder.ApplyConfiguration(new CourseEnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new LessonProgressConfiguration());
        modelBuilder.ApplyConfiguration(new CourseCategoryConfiguration());
    }
}