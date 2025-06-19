using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Interface for custom schema filters that specifically handle Ad hoc Unions.
/// </summary>
public interface IAdHocUnionSchemaFilter
{
   /// <summary>
   /// Applies customizations to the OpenAPI schema for an Ad hoc Union.
   /// </summary>
   /// <param name="schema">The OpenAPI schema to modify.</param>
   /// <param name="context">The schema filter context containing type information.</param>
   void Apply(OpenApiSchema schema, SchemaFilterContext context);
}
