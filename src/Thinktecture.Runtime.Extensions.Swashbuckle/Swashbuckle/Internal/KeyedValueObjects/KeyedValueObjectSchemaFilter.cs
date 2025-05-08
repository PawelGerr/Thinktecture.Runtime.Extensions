using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal.KeyedValueObjects;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class KeyedValueObjectSchemaFilter : IInternalKeyedValueObjectSchemaFilter
{
   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
      if (MetadataLookup.Find(context.Type) is not Metadata.Keyed.ValueObject metadata)
         throw new InvalidOperationException($"The type '{context.Type.FullName}' is not a keyed Value Object.");

      Apply(schema, context, metadata);
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, Metadata.Keyed.ValueObject metadata)
   {
      schema.Properties.Clear();
      schema.Required.Clear();

      var keySchema = context.SchemaGenerator.GenerateSchema(metadata.KeyType, context.SchemaRepository);

      schema.Type = keySchema.Type;
      schema.Format = keySchema.Format;
      schema.Minimum = keySchema.Minimum;
      schema.Maximum = keySchema.Maximum;

      // Otherwise the type gets: "additionalProperties": false
      schema.AdditionalPropertiesAllowed = true;
      schema.AdditionalProperties = null;
   }
}
