using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Snackis.Application;
using Snackis.Domain.Entities;
using Snackis.Infrastructure;
using Snackis.Infrastructure.Data;
using Snackis.Web.Components;
using Snackis.Web.Components.Account;
using System.Globalization;

namespace Snackis.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        //DI
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddProfiles();
        builder.Services.AddFileStorage(Path.Combine(builder.Environment.WebRootPath, "uploads"));

        builder.Services.AddHttpClient("SnackisApi", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
        });


        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
            .AddRoles<IdentityRole>()
            .AddErrorDescriber<SwedishIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        //Timestamp
        var svenska = new CultureInfo("sv-SE");
        CultureInfo.DefaultThreadCurrentCulture = svenska;
        CultureInfo.DefaultThreadCurrentUICulture = svenska;

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                await DbSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Seedningen misslyckades vid uppstart");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }
}
