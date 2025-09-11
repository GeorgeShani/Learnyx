using System.Text.Json;
using learnyx.Models.Entities;
using learnyx.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<PlanCourseAccess> PlanCourseAccesses { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new UserPlanConfiguration());
        modelBuilder.ApplyConfiguration(new PlanCourseAccessConfiguration());

        // Configure enum conversions to strings
        ConfigureEnumConversions(modelBuilder);

        // Configure JSON conversions for arrays
        ConfigureJsonConversions(modelBuilder);

        // Configure entity relationships
        ConfigureRelationships(modelBuilder);
    }

    private static void ConfigureEnumConversions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(e => e.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Course>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Plan>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<UserPlan>()
            .Property(e => e.Status)
            .HasConversion<string>();
    }

    private static void ConfigureJsonConversions(ModelBuilder modelBuilder)
    {
        // Configure JSON arrays for lesson attachments
        modelBuilder.Entity<Lesson>()
            .Property(e => e.Attachments)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null))
            .HasColumnType("nvarchar(max)");

        // Configure JSON arrays for submission attachments
        modelBuilder.Entity<Submission>()
            .Property(e => e.Attachments)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null))
            .HasColumnType("nvarchar(max)");

        // Configure JSON arrays for plan features
        modelBuilder.Entity<Plan>()
            .Property(e => e.Features)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null))
            .HasColumnType("nvarchar(max)");
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // User (Teacher) -> Course relationship
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(u => u.TeacherCourses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Course -> Lesson relationship
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Course -> Assignment relationship
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Assignment -> Submission relationship
        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // User (Student) -> Submission relationship
        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // User (Student) -> Enrollment relationship
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Course -> Enrollment relationship
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> UserPlan relationship
        modelBuilder.Entity<UserPlan>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPlans)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Plan -> UserPlan relationship
        modelBuilder.Entity<UserPlan>()
            .HasOne(up => up.Plan)
            .WithMany(p => p.UserPlans)
            .HasForeignKey(up => up.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Plan -> PlanCourseAccess relationship
        modelBuilder.Entity<PlanCourseAccess>()
            .HasOne(pca => pca.Plan)
            .WithMany(p => p.CourseAccess)
            .HasForeignKey(pca => pca.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Course -> PlanCourseAccess relationship
        modelBuilder.Entity<PlanCourseAccess>()
            .HasOne(pca => pca.Course)
            .WithMany(c => c.PlanAccess)
            .HasForeignKey(pca => pca.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure additional indexes for performance
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // User indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Role)
            .HasDatabaseName("IX_Users_Role");

        // Course indexes
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Category)
            .HasDatabaseName("IX_Courses_Category");

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Status)
            .HasDatabaseName("IX_Courses_Status");

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.TeacherId)
            .HasDatabaseName("IX_Courses_TeacherId");

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.PublishedAt)
            .HasDatabaseName("IX_Courses_PublishedAt");

        // Lesson indexes
        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.CourseId, l.OrderIndex })
            .HasDatabaseName("IX_Lessons_CourseId_OrderIndex");

        // Assignment indexes
        modelBuilder.Entity<Assignment>()
            .HasIndex(a => a.DueDate)
            .HasDatabaseName("IX_Assignments_DueDate");

        modelBuilder.Entity<Assignment>()
            .HasIndex(a => a.CourseId)
            .HasDatabaseName("IX_Assignments_CourseId");

        // Submission indexes
        modelBuilder.Entity<Submission>()
            .HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique()
            .HasDatabaseName("IX_Submissions_AssignmentId_StudentId");

        modelBuilder.Entity<Submission>()
            .HasIndex(s => s.SubmittedAt)
            .HasDatabaseName("IX_Submissions_SubmittedAt");
    }
}