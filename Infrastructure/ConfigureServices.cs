using Application.Common.Interfaces;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        //Register Token Provider service
        var tokenProvider = new TokenProviderService(configuration["JWT:ValidIssuer"] ?? "", configuration["JWT:ValidAudience"]??"", configuration["JWT:Secret"] ?? "");
        services.AddSingleton<ITokenProvider>(tokenProvider);

        //Add Database
        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite())
           .EnableSensitiveDataLogging());

        //Abstraction interface for application db context
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        //Configure Custom Application User
        services.AddDefaultIdentity<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        //Configure Identity
        services.Configure<IdentityOptions>(opts =>
        {
            opts.SignIn.RequireConfirmedEmail = true;
            opts.Password.RequiredLength = 6;
        });

        services.AddScoped<ApplicationDbContextInitialiser>();

        //Registered Services
        services.AddHostedService<CronJobFunctions>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }
}
