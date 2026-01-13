using Microsoft.EntityFrameworkCore;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Application.Mappings;
using UptimeChangeMonitor.Application.Services;
using UptimeChangeMonitor.Infrastructure.Data;
using UptimeChangeMonitor.Infrastructure.Messaging;
using UptimeChangeMonitor.Infrastructure.Repositories;
using UptimeChangeMonitor.Infrastructure.Services;

namespace UptimeChangeMonitor.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IMonitorRepository, MonitorRepository>();
        services.AddScoped<IUptimeCheckRepository, UptimeCheckRepository>();
        services.AddScoped<IChangeDetectionRepository, ChangeDetectionRepository>();

        // RabbitMQ
        services.AddSingleton<RabbitMQService>();
        services.AddScoped<MessagePublisher>();
        
        // Services
        services.AddScoped<IQueueService, Infrastructure.Services.QueueService>();
        services.AddScoped<IMonitorService, MonitorService>();
        services.AddScoped<IUptimeCheckService, UptimeCheckService>();
        services.AddScoped<IChangeDetectionService, ChangeDetectionService>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
