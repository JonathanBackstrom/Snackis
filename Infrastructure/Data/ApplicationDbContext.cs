using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;


namespace Snackis.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // => Set istället för { get; set; } för att slippa null varningarna och använda = null!
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Discussion> Discussions => Set<Discussion>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostImage> PostImages => Set<PostImage>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<BannedWord> BannedWords => Set<BannedWord>();
    public DbSet<ForumSettings> ForumSettings => Set<ForumSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
