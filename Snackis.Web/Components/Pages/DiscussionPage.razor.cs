using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Validation;
using Snackis.Domain.Enums;
using System.Security.Claims;

namespace Snackis.Web.Components.Pages;

public partial class DiscussionPage
{
    [CascadingParameter]
    public Task<AuthenticationState>? AuthState { get; set; }

    [Parameter]
    public int Id { get; set; }

    private DiscussionDetailDto? discussion;
    private string? userId;
    private bool isAdmin;
    private CreatePostDto newPost = new();
    private string? error;
    private int? replyToPostId;
    private int? reportPostId;
    private CreateReportDto newReport = new();
    private string? message;
    private IReadOnlyList<IBrowserFile> selectedFiles = [];
    private bool confirmDelete;
    private bool notFound;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState!;
        userId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        isAdmin = state.User.IsInRole("Admin");

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            discussion = await DiscussionService.GetDetailAsync(Id, userId);
        }
        catch (NotFoundException)
        {
            notFound = true;
            discussion = null;
        }
    }

    private void StartReply(int postId)
    {
        replyToPostId = postId;
        error = null;

        error = null;
        message = null;
    }

    private async Task DeleteClicked()
    {
        if (!confirmDelete)
        {
            confirmDelete = true;
            return;
        }

        var categoryId = discussion!.CategoryId;

        try
        {
            await DiscussionService.DeleteAsync(Id);
            Navigation.NavigateTo($"/categories/{categoryId}");
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
            confirmDelete = false;
        }
    }

    private async Task ToggleLock()
    {
        error = null;

        try
        {
            await DiscussionService.SetLockedAsync(Id, !discussion!.IsLocked);
            await LoadAsync();
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task HandleReply()
    {
        error = null;

        try
        {
            newPost.DiscussionId = Id;
            newPost.ParentPostId = replyToPostId;

            var postId = await PostService.CreateAsync(newPost, userId!);

            foreach (var fil in selectedFiles)
            {
                await using var stream = fil.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024);
                await PostService.AddImageAsync(postId, stream, fil.Name, fil.Size, userId!);
            }

            newPost = new();
            selectedFiles = [];
            replyToPostId = null;

            await LoadAsync();
        }
        catch (IOException)
        {
            error = "Inlägget skapades, men en bild kunde inte laddas upp.";
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private void StartReport(int postID)
    {
        reportPostId = postID;
        newReport = new() { PostId = postID };
        replyToPostId = null;

        error = null;
        message = null;
    }

    private async Task HandleReport()
    {
        error = null;
        try
        {
            await ReportService.CreateAsync(newReport, userId!);

            reportPostId = null;
            newReport = new();
            message = "Tack, din anmälan har skickats till moderatorerna.";
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private async Task HandleReact((int PostId, ReactionType type) reaction)
    {
        error = null;

        try
        {
            await ReactionService.ReactAsync(reaction.PostId, userId!, reaction.type);
            await LoadAsync();
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)
        {
            error = ex.Message;
        }
    }

    private void HandleFiles(InputFileChangeEventArgs e)
    {
        error = null;
        selectedFiles = [];

        try
        {
            var files = e.GetMultipleFiles(maximumFileCount: 4);

            foreach (var file in files)
            {
                ImageRules.Validate(file.Name, file.Size);
            }

            selectedFiles = files;
        }
        catch (BusinessRuleException ex)
        {
            error = ex.Message;
        }
    }
}