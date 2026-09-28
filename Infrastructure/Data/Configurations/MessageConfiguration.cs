using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(m => m.Sender).WithMany()
        .HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
    }
}
