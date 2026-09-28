using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snackis.Infrastructure.Data.Configurations;

internal class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.HasKey(cp => new { cp.ConversationId, cp.UserId });

        builder.HasOne(cp => cp.User).WithMany()
            .HasForeignKey(cp => cp.UserId).OnDelete(DeleteBehavior.Restrict);

    }
}
