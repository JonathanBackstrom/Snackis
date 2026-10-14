using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snackis.Application.Interfaces;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;
using Snackis.Infrastructure.Repositories;
using Snackis.Infrastructure.Storage;

namespace Snackis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection saknas.");

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ApplicationDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDiscussionRepository, DiscussionRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IBannedWordRepository, BannedWordRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IPostImageRepository, PostImageRepository>();

        return services;
    }

    public static IServiceCollection AddFileStorage(this IServiceCollection services, string rootPath)
    {
        services.AddSingleton<IFileStorage>(new LocalFileStorage(rootPath));
        return services;
    }
}
