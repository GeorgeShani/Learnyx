using learnyx.Models.Entities;
using learnyx.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class UserPlanConfiguration : IEntityTypeConfiguration<UserPlan>
{
    public void Configure(EntityTypeBuilder<UserPlan> builder)
    {
        builder.ToTable("UserPlans");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.UserId)
            .IsRequired();

        builder.Property(up => up.PlanId)
            .IsRequired();

        builder.Property(up => up.StartDate)
            .IsRequired();

        builder.Property(up => up.EndDate)
            .IsRequired();

        builder.Property(up => up.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(UserPlanStatus.ACTIVE);

        builder.Property(up => up.AmountPaid)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(up => up.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(up => up.TransactionId)
            .HasMaxLength(100);

        builder.Property(up => up.CancelledAt)
            .IsRequired(false);

        builder.Property(up => up.CancellationReason)
            .HasMaxLength(500);

        builder.Property(up => up.AutoRenew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.ReminderSentAt)
            .IsRequired(false);

        builder.Property(up => up.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(up => up.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(up => new { up.UserId, up.Status })
            .HasDatabaseName("IX_UserPlans_UserId_Status");

        builder.HasIndex(up => up.EndDate)
            .HasDatabaseName("IX_UserPlans_EndDate");

        builder.HasIndex(up => up.TransactionId)
            .HasDatabaseName("IX_UserPlans_TransactionId");
    }
}