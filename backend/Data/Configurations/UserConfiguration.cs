using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Password is nullable for social auth users
        builder.Property(u => u.Password)
            .HasMaxLength(255);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.AuthProvider)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Local");

        builder.Property(u => u.GoogleId)
            .HasMaxLength(100);

        builder.Property(u => u.FacebookId)
            .HasMaxLength(100);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Indexes for performance
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.GoogleId)
            .IsUnique()
            .HasFilter("[GoogleId] IS NOT NULL");

        builder.HasIndex(u => u.FacebookId)
            .IsUnique()
            .HasFilter("[FacebookId] IS NOT NULL");

        // Relationships
        builder.HasMany(u => u.TeacherCourses)
            .WithOne(c => c.Teacher)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UserPlans)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Submissions)
            .WithOne(s => s.Student)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}