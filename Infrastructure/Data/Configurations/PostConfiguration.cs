using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasOne(p => p.Author).WithMany(u => u.Posts)
        .HasForeignKey(p => p.AuthorId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Discussion)
       .WithMany(d => d.Posts)
       .HasForeignKey(p => p.DiscussionId)
       .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ParentPost)
       .WithMany(p => p.Replies)
       .HasForeignKey(p => p.ParentPostId)
       .OnDelete(DeleteBehavior.Restrict);
    }
}
