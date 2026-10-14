using Microsoft.Extensions.DependencyInjection;
using Snackis.Application.Interfaces;
using Snackis.Application.Services;

namespace Snackis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDiscussionService, DiscussionService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IBannedWordService, BannedWordService>();
        services.AddScoped<IReactionService, ReactionService>();


        services.AddSingleton<IProfanityFilter, ProfanityFilter>();
        return services;
    }
    public static IServiceCollection AddProfiles(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        return services;
    }
}
