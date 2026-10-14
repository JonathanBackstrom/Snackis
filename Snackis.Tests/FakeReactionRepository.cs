using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakeReactionRepository : IReactionRepository
{
    private readonly List<Reaction> reactions = [];
    private int nextId = 1;

    public FakeReactionRepository(params Reaction[] seed)
    {
        foreach (var reaction in seed)
        {
            Lagg(reaction);
        }
    }

    public Task<Reaction?> GetByPostAndUserAsync(int postId, string userId)
        => Task.FromResult(reactions.FirstOrDefault(r => r.PostId == postId && r.UserId == userId));

    public Task<IReadOnlyList<Reaction>> GetByDiscussionIdAsync(int discussionId)
        => Task.FromResult<IReadOnlyList<Reaction>>(reactions.ToList());

    public Task AddAsync(Reaction reaction)
    {
        Lagg(reaction);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Reaction reaction) => Task.CompletedTask;

    public Task DeleteAsync(int id)
    {
        var reaction = reactions.FirstOrDefault(r => r.Id == id);
        if (reaction is not null)
        {
            reactions.Remove(reaction);
        }

        return Task.CompletedTask;
    }

    public int Antal => reactions.Count;

    private void Lagg(Reaction reaction)
    {
        if (reaction.Id == 0)
        {
            reaction.Id = nextId++;
        }
        else if (reaction.Id >= nextId)
        {
            nextId = reaction.Id + 1;
        }

        reactions.Add(reaction);
    }
}
