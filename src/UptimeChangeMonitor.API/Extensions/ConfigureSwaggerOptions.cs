using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace UptimeChangeMonitor.API.Extensions;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "Uptime & Change Monitor API",
            Version = description.ApiVersion.ToString(),
            Description = "API para monitoramento de sites e detecção de mudanças de conteúdo",
            Contact = new OpenApiContact
            {
                Name = "Uptime Change Monitor",
                Email = "support@uptimemonitor.com"
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += " (Esta versão está deprecada)";
        }

        return info;
    }
}
