using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class ConversationRepository(IDbContextFactory<ApplicationDbContext> factory) : IConversationRepository
{
    public async Task<Conversation> AddAsync(Conversation conversation)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();
        return conversation;
    }

    public async Task<Message?> GetMessageAsync(int messageId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task DeleteMessageAsync(int messageId)
    {
        await using var context = await factory.CreateDbContextAsync();

        var message = await context.Messages.FindAsync(messageId);
        if (message is null) return;

        context.Messages.Remove(message);
        await context.SaveChangesAsync();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Messages.Add(message);
        await context.SaveChangesAsync();
        return message;
    }


    public async Task<Conversation?> FindDirectAsync(string userIdA, string userIdB)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c =>
                !c.IsGroup
                && c.Participants.Count == 2
                && c.Participants.Any(p => p.UserId == userIdA)
                && c.Participants.Any(p => p.UserId == userIdB));
    }

    public async Task<Conversation?> GetByIdAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Conversation>> GetForUserAsync(string userId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages)
            .Where(c => c.Participants.Any(p => p.UserId == userId
                && (p.HiddenAt == null
                    || c.Messages.Any(m => m.SentAt > p.HiddenAt))))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(int conversationId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task HideAsync(int conversationId, string userId)
    {
        await using var context = await factory.CreateDbContextAsync();

        var participant = await context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant is null) return;

        participant.HiddenAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(int conversationId, string userId)
    {
        await using var context = await factory.CreateDbContextAsync();

        var participant = await context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant is null) return;

        participant.LastReadAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
    public async Task AddParticipantAsync(ConversationParticipant participant)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.ConversationParticipants.Add(participant);
        await context.SaveChangesAsync();
        return;
    }

    public async Task RemoveParticipantAsync(int conversationId, string userId)
    {
        await using var context = await factory.CreateDbContextAsync();

        var participant = await context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant is null) return;

        context.ConversationParticipants.Remove(participant);
        await context.SaveChangesAsync();
    }
}
