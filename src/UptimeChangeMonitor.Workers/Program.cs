using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Application.Mappings;
using UptimeChangeMonitor.Application.Services;
using UptimeChangeMonitor.Infrastructure.Data;
using UptimeChangeMonitor.Infrastructure.Repositories;
using UptimeChangeMonitor.Workers.ChangeDetectionWorker;
using UptimeChangeMonitor.Workers.UptimeWorker;

var builder = Host.CreateApplicationBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMonitorRepository, MonitorRepository>();
builder.Services.AddScoped<IUptimeCheckRepository, UptimeCheckRepository>();
builder.Services.AddScoped<IChangeDetectionRepository, ChangeDetectionRepository>();

// Workers
builder.Services.AddHostedService<UptimeWorker>();
builder.Services.AddHostedService<ChangeDetectionWorker>();

var host = builder.Build();
host.Run();
