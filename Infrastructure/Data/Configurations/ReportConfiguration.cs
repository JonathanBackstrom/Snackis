using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(r => r.Reason).HasMaxLength(500);

        builder.HasOne(r => r.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reporter).WithMany()
            .HasForeignKey(r => r.ReporterId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.HandledBy).WithMany()
            .HasForeignKey(r => r.HandledById).OnDelete(DeleteBehavior.Restrict);
    }
}
