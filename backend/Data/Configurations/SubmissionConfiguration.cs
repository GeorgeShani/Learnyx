using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.Property(s => s.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.Attachments)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Score);

        builder.Property(s => s.Feedback)
            .HasColumnType("text");

        builder.Property(s => s.SubmittedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
    }   
}