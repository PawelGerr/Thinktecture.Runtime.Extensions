using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Interface for custom schema filters that specifically handle complex Value Objects.
/// </summary>
public interface IComplexValueObjectSchemaFilter
{
   /// <summary>
   /// Applies customizations to the OpenAPI schema for a complex Value Object.
   /// </summary>
   /// <param name="schema">The OpenAPI schema to modify.</param>
   /// <param name="context">The schema filter context containing type information.</param>
   void Apply(OpenApiSchema schema, SchemaFilterContext context);
}
