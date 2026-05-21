using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Nodes;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // Swashbuckle 10 eliminó la extensión IsDeprecated(); se replica su lógica original
        // (presencia de [Obsolete]) usando CustomAttributes(), que sigue disponible.
        operation.Deprecated |= apiDescription.CustomAttributes().OfType<ObsoleteAttribute>().Any();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse
                              ? "default"
                              : responseType.StatusCode.ToString();
            if (!operation.Responses.TryGetValue(responseKey, out var response) || response.Content is null)
            {
                continue;
            }

            foreach (var contentType in response.Content.Keys.ToArray())
            {
                if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters == null)
        {
            return;
        }

        // Microsoft.OpenApi v2 (Swashbuckle 9+) expone los parámetros como IOpenApiParameter
        // (de solo lectura para referencias); operamos sobre las instancias concretas mutables.
        foreach (var parameter in operation.Parameters.OfType<OpenApiParameter>())
        {
            var description = apiDescription.ParameterDescriptions
                                            .First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema is OpenApiSchema schema
                && schema.Default == null
                && description.DefaultValue != null
                && description.ModelMetadata is { } modelMetadata)
            {
                var json = JsonSerializer.Serialize(
                    description.DefaultValue,
                    modelMetadata.ModelType);
                schema.Default = JsonNode.Parse(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}