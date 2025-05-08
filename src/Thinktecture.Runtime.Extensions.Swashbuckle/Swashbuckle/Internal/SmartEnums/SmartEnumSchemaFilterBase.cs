using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal.SmartEnums;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public abstract class SmartEnumSchemaFilterBase : IInternalSmartEnumSchemaFilter
{
   private readonly IOpenApiValueFactoryProvider _valueFactoryProvider;
   private readonly ISmartEnumSchemaExtension _extension;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   protected SmartEnumSchemaFilterBase(
      IServiceProvider serviceProvider,
      IOpenApiValueFactoryProvider valueFactoryProvider,
      IOptions<ThinktectureSchemaFilterOptions> options)
   {
      _valueFactoryProvider = valueFactoryProvider;
      _extension = options.Value.SmartEnumSchemaExtension.CreateSchemaExtension(serviceProvider);
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
      if (MetadataLookup.Find(context.Type) is not Metadata.Keyed.SmartEnum metadata)
         throw new InvalidOperationException($"The type '{context.Type.FullName}' is not a Smart Enum.");

      Apply(schema, context, metadata);
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, Metadata.Keyed.SmartEnum metadata)
   {
      schema.Properties.Clear();
      schema.Required.Clear();

      var items = GetItems(metadata.Type, metadata.KeyType, metadata.GetItems());

      SetItems(schema, items);

      var keySchema = context.SchemaGenerator.GenerateSchema(metadata.KeyType, context.SchemaRepository);
      CopyPropertiesFromKeyTypeSchema(schema, context, keySchema);

      _extension.Apply(schema, context, metadata.GetItems());
   }

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   protected abstract void SetItems(OpenApiSchema schema, IReadOnlyList<SmartEnumItem> items);

   /// <summary>
   /// Creates a list of OpenAPI values for the items of a Smart Enum.
   /// </summary>
   /// <param name="type">The type of the Smart Enum.</param>
   /// <param name="keyType">The key type of the Smart Enum.</param>
   /// <param name="items">The collection of items available in the Smart Enum.</param>
   /// <returns>A list of OpenAPI values representing the Smart Enum items.</returns>
   /// <exception cref="NotSupportedException">Thrown when the key type is not supported.</exception>
   protected virtual IReadOnlyList<SmartEnumItem> GetItems(
      Type type,
      Type keyType,
      IEnumerable<object> items)
   {
      var (valueFactory, keySelector) = DetermineValueFactoryAndKeySelector(type, keyType);

      return items.Select(item =>
                  {
                     var key = keySelector(item);
                     var value = valueFactory.CreateOpenApiValue(key);

                     return new SmartEnumItem(item, value);
                  })
                  .ToList();
   }

   /// <summary>
   /// Determines a value factory and key selector for the given type and key type.
   /// </summary>
   /// <param name="type">Type.</param>
   /// <param name="keyType">The key type.</param>
   /// <exception cref="NotSupportedException">Thrown when the key type is not supported.</exception>
   protected virtual KeyPropertyValueFactory DetermineValueFactoryAndKeySelector(
      Type type,
      Type keyType)
   {
      if (MetadataLookup.Find(type) is not Metadata.Keyed metadata)
         throw new NotSupportedException($"The type '{type.FullName}' is not supported.");

      return DetermineValueFactoryAndKeySelector(metadata);
   }

   private KeyPropertyValueFactory DetermineValueFactoryAndKeySelector(
      Metadata.Keyed metadata)
   {
      if (_valueFactoryProvider.TryGet(metadata.KeyType, out var factory))
         return new(factory, metadata.GetKey);

      if (MetadataLookup.Find(metadata.KeyType) is not Metadata.Keyed keyTypeMetadata)
         throw new NotSupportedException($"The key type '{metadata.KeyType.FullName}' of type '{metadata.Type.FullName}' is not supported.");

      var keyPropertyValueFactory = DetermineValueFactoryAndKeySelector(keyTypeMetadata);

      return keyPropertyValueFactory with { KeySelector = o => keyPropertyValueFactory.KeySelector(metadata.GetKey(o)) };
   }

   /// <summary>
   /// Copies relevant properties from the key type schema to the provided schema.
   /// </summary>
   /// <param name="schema">The target schema to which properties are copied.</param>
   /// <param name="context">The current schema filter context.</param>
   /// <param name="keySchema">The schema of the key type from which properties are copied.</param>
   protected virtual void CopyPropertiesFromKeyTypeSchema(
      OpenApiSchema schema,
      SchemaFilterContext context,
      OpenApiSchema keySchema)
   {
      schema.Type = keySchema.Type;
      schema.Format = keySchema.Format;
      schema.Minimum = keySchema.Minimum;
      schema.Maximum = keySchema.Maximum;

      // Otherwise the type gets: "additionalProperties": false
      schema.AdditionalPropertiesAllowed = true;
      schema.AdditionalProperties = null;
   }
}
