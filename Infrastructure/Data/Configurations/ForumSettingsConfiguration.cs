using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class ForumSettingsConfiguration : IEntityTypeConfiguration<ForumSettings>
{
    public void Configure(EntityTypeBuilder<ForumSettings> builder)
    {
        builder.Property(f => f.Title).HasMaxLength(100);
    }
}
