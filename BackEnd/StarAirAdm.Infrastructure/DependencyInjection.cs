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
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<StarAirAdm.Application.Common.Interfaces.IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Phase 2 services
        services.AddScoped<INotificationService, NotificationService>();


        return services;
    }
}
