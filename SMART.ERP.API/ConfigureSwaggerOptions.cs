using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            if (description.ApiVersion.ToString() == "1.0")
            {
                options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                   Title = "SMART ERP",
                   Description = "Una API web de ASP.NET Core para gestionar el ERP del lado administrativo.",
                   Version = description.ApiVersion.ToString(),
                   Contact = new OpenApiContact
                   {
                       Name = "Contacto",
                       Url = new Uri("https://github.com/Smart-Business-HN")
                   },
                });
            }
            else if (description.ApiVersion.ToString() == "2.0")
            {
                options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                    Title = "SMART ERP",
                    Description = "Una API web de ASP.NET Core para gestionar el E-Commerce.",
                    Version = description.ApiVersion.ToString(),
                    Contact = new OpenApiContact
                    {
                        Name = "Contacto",
                        Url = new Uri("https://github.com/Smart-Business-HN")
                    },
                });
            }
            else
            {
                options.SwaggerDoc(
               description.GroupName,
               new OpenApiInfo()
               {
                   Title = "SMART ERP",
                   Description = "Nueva API VERSION Reemplace este dato en SMART.ERP.API/ConfigureSwaggerOptions.cs",
                   Version = description.ApiVersion.ToString(),
                   Contact = new OpenApiContact
                   {
                       Name = "Contacto",
                       Url = new Uri("https://github.com/Smart-Business-HN")
                   },
               });
            }

        }
    }
}