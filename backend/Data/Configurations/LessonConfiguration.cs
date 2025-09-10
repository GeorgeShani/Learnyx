using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace learnyx.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(l => l.Attachments)
            .HasColumnType("nvarchar(max)");

        builder.Property(l => l.OrderIndex)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
    }
}