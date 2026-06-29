using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
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
   private readonly OrphanKeyTypeSchemaRegistry _orphanKeyTypeSchemaRegistry;
   private readonly bool _clearAllOf;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public KeyedValueObjectSchemaFilter(
      IServiceProvider serviceProvider,
      IOptions<ThinktectureSchemaFilterOptions> options)
   {
      _orphanKeyTypeSchemaRegistry = serviceProvider.GetRequiredService<OrphanKeyTypeSchemaRegistry>();
      _clearAllOf = options.Value.ClearAllOfOnKeyedTypes;
   }

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
      schema.Properties?.Clear();
      schema.Required?.Clear();

      if (_clearAllOf)
         schema.AllOf?.Clear();

      // Resolve the key schema (an enum key is resolved to its referenced component so the real wire
      // representation is used) and copy the key schema's scalar facets onto this schema.
      var keySchema = KeySchemaResolver.Resolve(context, metadata.KeyType, out var keyTypeComponentId);

      // When the key is exposed as a public member (e.g. [ValueObject<TEnum>(KeyMemberKind = Property,
      // KeyMemberAccessModifier = Public)]) Swashbuckle walks it while generating the type and registers a
      // componentized key type (e.g. an enum) in the main repository before this filter runs. Clearing the
      // properties above leaves that component orphaned. Record it so it can be removed afterwards if it ends
      // up unreferenced (see "RemoveOrphanedKeyTypeSchemasDocumentFilter"). For the default (non-public key)
      // Value Object nothing is registered in the main repository, so the document filter leaves it untouched.
      if (keyTypeComponentId is not null)
         _orphanKeyTypeSchemaRegistry.Register(context.SchemaRepository, keyTypeComponentId);

      schema.Type = keySchema.Type;
      schema.Format = keySchema.Format;
      schema.Minimum = keySchema.Minimum;
      schema.Maximum = keySchema.Maximum;

      // Otherwise the type gets: "additionalProperties": false
      schema.AdditionalPropertiesAllowed = true;
      schema.AdditionalProperties = null;
   }
}
