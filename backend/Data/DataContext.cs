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
        
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new UserPlanConfiguration());
        modelBuilder.ApplyConfiguration(new PlanCourseAccessConfiguration());
        
        modelBuilder.Entity<User>()
            .Property(e => e.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Course>()
            .Property(e => e.Status)
            .HasConversion<string>();

        // Configure JSON arrays for attachments
        modelBuilder.Entity<Lesson>()
            .Property(e => e.Attachments)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)
            );

        modelBuilder.Entity<Submission>()
            .Property(e => e.Attachments)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)
            );

        // Configure relationships
        ConfigureRelationships(modelBuilder);
    }
    
    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // User relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.TeacherCourses)
            .WithOne(c => c.Teacher)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserPlans)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Submissions)
            .WithOne(s => s.Student)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Course relationships
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Assignments)
            .WithOne(a => a.Course)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.PlanAccess)
            .WithOne(pca => pca.Course)
            .HasForeignKey(pca => pca.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Assignment -> Submission relationship
        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Plan relationships
        modelBuilder.Entity<Plan>()
            .HasMany(p => p.UserPlans)
            .WithOne(up => up.Plan)
            .HasForeignKey(up => up.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Plan>()
            .HasMany(p => p.CourseAccess)
            .WithOne(pca => pca.Plan)
            .HasForeignKey(pca => pca.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Category);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Status);

        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.CourseId, l.OrderIndex });

        modelBuilder.Entity<Assignment>()
            .HasIndex(a => a.DueDate);

        modelBuilder.Entity<Submission>()
            .HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique();

        modelBuilder.Entity<UserPlan>()
            .HasIndex(up => new { up.UserId, up.PlanId })
            .IsUnique();

        modelBuilder.Entity<PlanCourseAccess>()
            .HasIndex(pca => new { pca.PlanId, pca.CourseId })
            .IsUnique();

        modelBuilder.Entity<UserPlan>()
            .HasIndex(up => up.EndDate);
    }
}