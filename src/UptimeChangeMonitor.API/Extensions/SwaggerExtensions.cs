using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace UptimeChangeMonitor.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Incluir comentários XML (opcional, se tiver)
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        // Configurar Swagger para múltiplas versões
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            
            foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(d => d.ApiVersion))
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Uptime & Change Monitor API {description.GroupName.ToUpperInvariant()}");
            }
            
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
            options.DocumentTitle = "Uptime & Change Monitor API";
            options.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}
