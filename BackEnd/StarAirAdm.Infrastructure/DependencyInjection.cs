using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StarAirAdm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Infrastructure.Services;
using StarAirAdm.Application.Models;

namespace StarAirAdm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<StarAirAdm.Infrastructure.Data.Interceptors.AuditInterceptor>();

        // We use IServiceProvider to resolve AuditInterceptor because it depends on ICurrentUserService which is scoped
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .AddInterceptors(sp.GetRequiredService<StarAirAdm.Infrastructure.Data.Interceptors.AuditInterceptor>());
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        // Phase 2 services
        services.AddScoped<IImSafeService, ImSafeService>();
        services.AddScoped<IPaveService, PaveService>();
        services.AddScoped<IDecideService, DecideService>();
        services.AddScoped<ISmartWatchService, SmartWatchService>();
        services.AddScoped<IKneeboardService, KneeboardService>();
        services.AddScoped<IChecklistService, ChecklistService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<INotificationService, NotificationService>();


        return services;
    }
}
