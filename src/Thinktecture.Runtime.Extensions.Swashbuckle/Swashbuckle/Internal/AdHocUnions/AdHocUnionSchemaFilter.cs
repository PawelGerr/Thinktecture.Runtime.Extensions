using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal.AdHocUnions;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class AdHocUnionSchemaFilter : IInternalAdHocUnionSchemaFilter
{
   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
      if (MetadataLookup.Find(context.Type) is not Metadata.AdHocUnion metadata)
         throw new InvalidOperationException($"The type '{context.Type.FullName}' is not an Ad Hoc Union.");

      Apply(schema, context, metadata);
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, Metadata.AdHocUnion metadata)
   {
      schema.Properties.Clear();

      schema.Type = null;
      schema.OneOf = metadata.MemberTypes
                             .Select(t => context.SchemaGenerator.GenerateSchema(t, context.SchemaRepository))
                             .ToList();

      // Otherwise the type gets: "additionalProperties": false
      schema.AdditionalPropertiesAllowed = true;
      schema.AdditionalProperties = null;
   }
}
