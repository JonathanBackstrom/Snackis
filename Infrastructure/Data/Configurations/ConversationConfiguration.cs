using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasOne(c => c.Owner).WithMany()
            .HasForeignKey(c => c.OwnerId).OnDelete(DeleteBehavior.Restrict);
    }
}
