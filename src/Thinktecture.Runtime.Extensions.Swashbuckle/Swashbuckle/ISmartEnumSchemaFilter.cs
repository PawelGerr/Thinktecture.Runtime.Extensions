using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Interface for custom schema filters that specifically handle Smart Enums.
/// </summary>
public interface ISmartEnumSchemaFilter
{
   /// <summary>
   /// Applies customizations to the OpenAPI schema for a Smart Enum.
   /// </summary>
   /// <param name="schema">The OpenAPI schema to modify.</param>
   /// <param name="context">The schema filter context containing type information.</param>
   void Apply(OpenApiSchema schema, SchemaFilterContext context);
}
