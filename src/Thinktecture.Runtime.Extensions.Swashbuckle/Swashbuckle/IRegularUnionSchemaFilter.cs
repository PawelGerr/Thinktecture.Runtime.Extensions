using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Interface for custom schema filters that specifically handle Regular Unions.
/// </summary>
public interface IRegularUnionSchemaFilter
{
   /// <summary>
   /// Applies customizations to the OpenAPI schema for a Regular Union.
   /// </summary>
   /// <param name="schema">The OpenAPI schema to modify.</param>
   /// <param name="context">The schema filter context containing type information.</param>
   void Apply(OpenApiSchema schema, SchemaFilterContext context);
}
