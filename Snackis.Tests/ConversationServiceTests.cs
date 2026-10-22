using Snackis.Application.Exceptions;
using Snackis.Application.Services;
using Snackis.Domain.Entities;

namespace Snackis.Tests;

public class ConversationServiceTests
{
    private const string Anna = "anna-id";
    private const string Bo = "bo-id";
    private const string Cesar = "cesar-id";

    private static Conversation Konversation(int id, params string[] deltagare) => new()
    {
        Id = id,
        IsGroup = false,
        OwnerId = deltagare[0],
        Participants = [.. deltagare.Select(d => new ConversationParticipant { ConversationId = id, UserId = d })]
    };

    private static Message Meddelande(int id, int conversationId, string senderId, string body) => new()
    {
        Id = id,
        ConversationId = conversationId,
        SenderId = senderId,
        Body = body
    };

    [Fact]
    public async Task DeleteMessageAsync_EgetMeddelande_TasBort()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo))
            .MedMeddelanden(Meddelande(1, 1, Anna, "Hej"));

        var service = new ConversationService(repo, new FakeUserRepository());

        await service.DeleteMessageAsync(1, 1, Anna);

        Assert.Empty(repo.Meddelanden);
    }

    [Fact]
    public async Task DeleteMessageAsync_AnnansMeddelande_KastarBusinessRuleException()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo))
            .MedMeddelanden(Meddelande(1, 1, Bo, "Hej"));

        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.DeleteMessageAsync(1, 1, Anna));

        Assert.Single(repo.Meddelanden);
    }

    [Fact]
    public async Task DeleteMessageAsync_IckeDeltagare_KastarNotFoundException()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo))
            .MedMeddelanden(Meddelande(1, 1, Anna, "Hej"));

        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteMessageAsync(1, 1, Cesar));

        Assert.Single(repo.Meddelanden);
    }

    [Fact]
    public async Task DeleteMessageAsync_MeddelandeIAnnanKonversation_KastarNotFoundException()
    {
        var repo = new FakeConversationRepository(
                Konversation(1, Anna, Bo),
                Konversation(2, Anna, Cesar))
            .MedMeddelanden(Meddelande(5, 2, Anna, "Hemligt"));

        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteMessageAsync(1, 5, Anna));

        Assert.Single(repo.Meddelanden);
    }

    [Fact]
    public async Task HideAsync_PrivatKonversation_SatterHiddenAt()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo));
        var service = new ConversationService(repo, new FakeUserRepository());

        await service.HideAsync(1, Anna);

        var kvar = await repo.GetByIdAsync(1);
        Assert.NotNull(kvar!.Participants.First(p => p.UserId == Anna).HiddenAt);
        Assert.Null(kvar.Participants.First(p => p.UserId == Bo).HiddenAt);
    }

    [Fact]
    public async Task HideAsync_Grupp_DoljsUtanAttDeltagandetPaverkas()
    {
        var grupp = Konversation(1, Anna, Bo);
        grupp.IsGroup = true;

        var repo = new FakeConversationRepository(grupp);
        var service = new ConversationService(repo, new FakeUserRepository());

        await service.HideAsync(1, Bo);

        var kvar = await repo.GetByIdAsync(1);
        Assert.NotNull(kvar!.Participants.First(p => p.UserId == Bo).HiddenAt);
        Assert.Contains(kvar.Participants, p => p.UserId == Bo);
    }

    [Fact]
    public async Task HideAsync_IckeDeltagare_KastarNotFoundException()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo));
        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.HideAsync(1, Cesar));
    }

    [Fact]
    public async Task LeaveGroupAsync_Deltagare_TasBortUrGruppen()
    {
        var grupp = Konversation(1, Anna, Bo);
        grupp.IsGroup = true;

        var repo = new FakeConversationRepository(grupp);
        var service = new ConversationService(repo, new FakeUserRepository());

        await service.LeaveGroupAsync(1, Bo);

        var kvar = await repo.GetByIdAsync(1);
        Assert.DoesNotContain(kvar!.Participants, p => p.UserId == Bo);
        Assert.Contains(kvar.Participants, p => p.UserId == Anna);
    }

    [Fact]
    public async Task LeaveGroupAsync_Agaren_KastarBusinessRuleException()
    {
        var grupp = Konversation(1, Anna, Bo);
        grupp.IsGroup = true;

        var repo = new FakeConversationRepository(grupp);
        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.LeaveGroupAsync(1, Anna));

        var kvar = await repo.GetByIdAsync(1);
        Assert.Equal(2, kvar!.Participants.Count);
    }

    [Fact]
    public async Task LeaveGroupAsync_PrivatKonversation_KastarBusinessRuleException()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo));
        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.LeaveGroupAsync(1, Bo));
    }

    [Fact]
    public async Task LeaveGroupAsync_IckeDeltagare_KastarNotFoundException()
    {
        var grupp = Konversation(1, Anna, Bo);
        grupp.IsGroup = true;

        var repo = new FakeConversationRepository(grupp);
        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.LeaveGroupAsync(1, Cesar));
    }

    [Fact]
    public async Task DeleteMessageAsync_OkantMeddelande_KastarNotFoundException()
    {
        var repo = new FakeConversationRepository(Konversation(1, Anna, Bo));

        var service = new ConversationService(repo, new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteMessageAsync(1, 99, Anna));
    }
}
