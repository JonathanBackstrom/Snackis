using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using System.Security.Claims;

namespace Snackis.Web.Components.Pages;

public partial class ConversationPage
{
    [Parameter]
    public int Id { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState>? AuthState { get; set; }

    private ConversationDetailDto? conversation;
    private SendMessageDto newMessage = new();
    private string? userId;
    private string? error;
    private string searchTerm = string.Empty;
    private IReadOnlyList<ParticipantDto>? searchResults;
    private CreateGroupDto newGroup = new();
    private bool notFound;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState!;
        userId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        newMessage.ConversationId = Id;

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            conversation = await ConversationService.GetAsync(Id, userId!);
        }
        catch (NotFoundException ex)
        {
            notFound = true;
            error = ex.Message;
        }
    }

    private async Task HandleSend()
    {
        error = null;

        try
        {
            await ConversationService.SendAsync(newMessage, userId!);

            newMessage = new() { ConversationId = Id };
            await LoadAsync();
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task Search()
    {
        error = null;

        try
        {
            searchResults = await ConversationService.SearchInviteCandidatesAsync(Id, userId!, searchTerm);
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task Invite(string invitedUserId)
    {
        error = null;

        try
        {
            await ConversationService.InviteAsync(Id, userId!, invitedUserId);

            searchTerm = string.Empty;
            searchResults = null;

            await LoadAsync();
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task RemoveParticipant(string userIdToRemove)
    {
        error = null;

        try
        {
            await ConversationService.RemoveParticipantAsync(Id, userId!, userIdToRemove);
            await LoadAsync();
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task CreateGroupFromDirect(string invitedUserId)
    {
        error = null;

        try
        {
            var id = await ConversationService.CreateGroupFromDirectAsync(Id, userId!, newGroup, invitedUserId);
            Navigation.NavigateTo($"/account/messages/{id}");
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }
}