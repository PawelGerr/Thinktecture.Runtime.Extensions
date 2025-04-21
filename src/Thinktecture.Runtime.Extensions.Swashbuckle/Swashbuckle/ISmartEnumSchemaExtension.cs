using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Interface for extending Smart Enum schema generation with additional OpenAPI extensions.
/// </summary>
public interface ISmartEnumSchemaExtension
{
   /// <summary>
   /// Writes additional OpenAPI extensions to the schema for Smart Enum.
   /// </summary>
   /// <param name="schema">The OpenAPI schema to which extensions will be added.</param>
   /// <param name="context">The schema filter context containing type information.</param>
   /// <param name="items">The collection of Smart Enum items.</param>
   void Apply(OpenApiSchema schema, SchemaFilterContext context, IEnumerable<object> items);
}
