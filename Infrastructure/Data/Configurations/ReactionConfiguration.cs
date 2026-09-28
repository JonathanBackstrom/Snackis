using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasOne(r => r.User).WithMany()
        .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.PostId, r.UserId })
        .IsUnique();

        builder.HasOne(r => r.Post)
        .WithMany(p => p.Reactions)
        .HasForeignKey(r => r.PostId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
