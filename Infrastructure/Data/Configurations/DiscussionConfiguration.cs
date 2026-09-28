using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.Property(d => d.Title).HasMaxLength(200);

        builder.HasOne(d => d.Category)
       .WithMany(c => c.Discussions)
       .HasForeignKey(d => d.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Author)
        .WithMany(u => u.Discussions)
        .HasForeignKey(d => d.AuthorId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
