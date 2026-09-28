using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snackis.Domain.Entities;

namespace Snackis.Infrastructure.Data.Configurations;

internal class BannedWordConfiguration : IEntityTypeConfiguration<BannedWord>
{
    public void Configure(EntityTypeBuilder<BannedWord> builder)
    {
        builder.Property(b => b.Word).HasMaxLength(100);
    }
}
