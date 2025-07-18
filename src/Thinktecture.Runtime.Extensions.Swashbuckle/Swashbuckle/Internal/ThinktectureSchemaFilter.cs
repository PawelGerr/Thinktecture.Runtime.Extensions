using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;
using Thinktecture.Swashbuckle.Internal.AdHocUnions;
using Thinktecture.Swashbuckle.Internal.ComplexValueObjects;
using Thinktecture.Swashbuckle.Internal.KeyedValueObjects;
using Thinktecture.Swashbuckle.Internal.SmartEnums;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class ThinktectureSchemaFilter : ISchemaFilter
{
   private readonly IKeylessSmartEnumSchemaFilter _keylessSmartEnumSchemaFilter;
   private readonly ISmartEnumSchemaFilter _smartEnumSchemaFilter;
   private readonly IKeyedValueObjectSchemaFilter _keyedValueObjectSchemaFilter;
   private readonly IComplexValueObjectSchemaFilter _complexValueObjectSchemaFilter;
   private readonly IAdHocUnionSchemaFilter _adHocUnionSchemaFilter;
   private readonly IRegularUnionSchemaFilter _regularUnionSchemaFilter;
   private readonly SwaggerGenOptions _swaggerGenOptions;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public ThinktectureSchemaFilter(
      IServiceProvider serviceProvider,
      IKeylessSmartEnumSchemaFilter keylessSmartEnumSchemaFilter,
      IKeyedValueObjectSchemaFilter keyedValueObjectSchemaFilter,
      IComplexValueObjectSchemaFilter complexValueObjectSchemaFilter,
      IAdHocUnionSchemaFilter adHocUnionSchemaFilter,
      IRegularUnionSchemaFilter regularUnionSchemaFilter,
      IOptions<ThinktectureSchemaFilterOptions> options,
      SwaggerGenOptions swaggerGenOptions)
   {
      _smartEnumSchemaFilter = options.Value.SmartEnumSchemaFilter.CreateSchemaFilter(serviceProvider);
      _keylessSmartEnumSchemaFilter = keylessSmartEnumSchemaFilter;
      _keyedValueObjectSchemaFilter = keyedValueObjectSchemaFilter;
      _complexValueObjectSchemaFilter = complexValueObjectSchemaFilter;
      _adHocUnionSchemaFilter = adHocUnionSchemaFilter;
      _regularUnionSchemaFilter = regularUnionSchemaFilter;
      _swaggerGenOptions = swaggerGenOptions;
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
      // Wrapper made by UseAllOfToExtendReferenceSchemas
      if (schema.Type is null)
         return;

      if (TryHandleModelBoundType(schema, context))
         return;

      if (TryHandleTypeWithObjectFactory(schema, context))
         return;

      var metadata = MetadataLookup.Find(context.Type);

      metadata?.Switch(
         (Filter: this, schema, context),
         keylessSmartEnum: static (state, smartEnumMetadata) => state.Filter.Apply(state.schema, state.context, smartEnumMetadata),
         keyedSmartEnum: static (state, smartEnumMetadata) => state.Filter.Apply(state.schema, state.context, smartEnumMetadata),
         keyedValueObject: static (state, keyedValueObjectMetadata) => state.Filter.Apply(state.schema, state.context, keyedValueObjectMetadata),
         complexValueObject: static (state, complexValueObjectMetadata) => state.Filter.Apply(state.schema, state.context, complexValueObjectMetadata),
         adHocUnion: static (state, adHocUnionMetadata) => state.Filter.Apply(state.schema, state.context, adHocUnionMetadata),
         regularUnion: static (state, regularUnionMetadata) => state.Filter.Apply(state.schema, state.context, regularUnionMetadata));
   }

   internal static Type GetSerializationType(Type type)
   {
      var metadata = MetadataLookup.FindMetadataForConversion(
         type,
         f => f.UseForSerialization.HasFlag(SerializationFrameworks.Json),
         _ => false);

      return metadata is null ? type : type.NormalizeStructType(metadata.Value.KeyType);
   }

   private bool TryHandleTypeWithObjectFactory(
      OpenApiSchema schema,
      SchemaFilterContext context)
   {
      var serializationType = GetSerializationType(context.Type);

      // Return to prevent infinite recursion
      if (context.Type == serializationType)
         return false;

      var newSchema = context.SchemaGenerator.GenerateSchema(serializationType, context.SchemaRepository);
      CopyProperties(newSchema, schema);

      return true;
   }

   private bool TryHandleModelBoundType(
      OpenApiSchema schema,
      SchemaFilterContext context)
   {
      var type = context.Type;

      if (!type.IsGenericType)
         return false;

      var genericTypeDefinition = type.IsGenericTypeDefinition
                                     ? type
                                     : type.GetGenericTypeDefinition();

      if (genericTypeDefinition != typeof(BoundParameter<,>))
         return false;

      var originalType = type.GetGenericArguments()[0];
      type = type.GetGenericArguments()[1];

      if (type == context.Type)
         return false;

      var underlyingTypeSchema = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
      CopyProperties(underlyingTypeSchema, schema);

      var originalTypeSchema = context.SchemaGenerator.GenerateSchema(originalType, context.SchemaRepository);

      if (originalTypeSchema.Reference is null
          || context.SchemaRepository.Schemas.TryGetValue(originalTypeSchema.Reference.Id, out originalTypeSchema))
      {
         // We want to keep the original schema's title and description
         schema.Title = String.IsNullOrWhiteSpace(originalTypeSchema.Title) ? schema.Title : originalTypeSchema.Title;
         schema.Description = String.IsNullOrWhiteSpace(originalTypeSchema.Description) ? schema.Description : originalTypeSchema.Description;
      }

      return true;
   }

   private void CopyProperties(OpenApiSchema sourceSchema, OpenApiSchema targetSchema)
   {
      // We want to keep the original schema's title and description
      targetSchema.Title = String.IsNullOrWhiteSpace(targetSchema.Title) ? sourceSchema.Title : targetSchema.Title;
      targetSchema.Description = String.IsNullOrWhiteSpace(targetSchema.Description) ? sourceSchema.Description : targetSchema.Description;

      targetSchema.Type = sourceSchema.Type;
      targetSchema.Format = sourceSchema.Format;
      targetSchema.Maximum = sourceSchema.Maximum;
      targetSchema.ExclusiveMaximum = sourceSchema.ExclusiveMaximum;
      targetSchema.Minimum = sourceSchema.Minimum;
      targetSchema.ExclusiveMinimum = sourceSchema.ExclusiveMinimum;
      targetSchema.MaxLength = sourceSchema.MaxLength;
      targetSchema.MinLength = sourceSchema.MinLength;
      targetSchema.Pattern = sourceSchema.Pattern;
      targetSchema.MultipleOf = sourceSchema.MultipleOf;
      targetSchema.Default = sourceSchema.Default;
      targetSchema.ReadOnly = sourceSchema.ReadOnly;
      targetSchema.WriteOnly = sourceSchema.WriteOnly;
      targetSchema.AllOf = sourceSchema.AllOf != null ? new List<OpenApiSchema>(sourceSchema.AllOf) : null;
      targetSchema.OneOf = sourceSchema.OneOf != null ? new List<OpenApiSchema>(sourceSchema.OneOf) : null;
      targetSchema.AnyOf = sourceSchema.AnyOf != null ? new List<OpenApiSchema>(sourceSchema.AnyOf) : null;
      targetSchema.Not = sourceSchema.Not;
      targetSchema.Required = sourceSchema.Required != null ? new HashSet<string>(sourceSchema.Required) : null;
      targetSchema.Items = sourceSchema.Items;
      targetSchema.MaxItems = sourceSchema.MaxItems;
      targetSchema.MinItems = sourceSchema.MinItems;
      targetSchema.UniqueItems = sourceSchema.UniqueItems;
      targetSchema.Properties = sourceSchema.Properties != null ? new Dictionary<string, OpenApiSchema>(sourceSchema.Properties) : null;
      targetSchema.MaxProperties = sourceSchema.MaxProperties;
      targetSchema.MinProperties = sourceSchema.MinProperties;
      targetSchema.AdditionalPropertiesAllowed = sourceSchema.AdditionalPropertiesAllowed;
      targetSchema.AdditionalProperties = sourceSchema.AdditionalProperties;
      targetSchema.Discriminator = sourceSchema.Discriminator;
      targetSchema.Example = sourceSchema.Example;
      targetSchema.Enum = sourceSchema.Enum != null ? new List<IOpenApiAny>(sourceSchema.Enum) : null;
      targetSchema.Nullable = sourceSchema.Nullable;
      targetSchema.ExternalDocs = sourceSchema.ExternalDocs;
      targetSchema.Deprecated = sourceSchema.Deprecated;
      targetSchema.Xml = sourceSchema.Xml;
      targetSchema.Extensions = sourceSchema.Extensions != null ? new Dictionary<string, IOpenApiExtension>(sourceSchema.Extensions) : null;
      targetSchema.UnresolvedReference = sourceSchema.UnresolvedReference;
      targetSchema.Reference = sourceSchema.Reference;
      targetSchema.Annotations = sourceSchema.Annotations != null ? new Dictionary<string, object>(sourceSchema.Annotations) : null;
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.KeylessSmartEnum metadata)
   {
      _keylessSmartEnumSchemaFilter.Apply(schema, context);
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.Keyed.SmartEnum metadata)
   {
      if (_smartEnumSchemaFilter is IInternalSmartEnumSchemaFilter internalFilter)
      {
         internalFilter.Apply(schema, context, metadata);
      }
      else
      {
         _smartEnumSchemaFilter.Apply(schema, context);
      }
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.Keyed.ValueObject metadata)
   {
      if (_keyedValueObjectSchemaFilter is IInternalKeyedValueObjectSchemaFilter internalFilter)
      {
         internalFilter.Apply(schema, context, metadata);
      }
      else
      {
         _keyedValueObjectSchemaFilter.Apply(schema, context);
      }
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.ComplexValueObject metadata)
   {
      if (_complexValueObjectSchemaFilter is IInternalComplexValueObjectSchemaFilter internalFilter)
      {
         internalFilter.Apply(schema, context, metadata);
      }
      else
      {
         _complexValueObjectSchemaFilter.Apply(schema, context);
      }
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.AdHocUnion metadata)
   {
      if (_adHocUnionSchemaFilter is IInternalAdHocUnionSchemaFilter internalFilter)
      {
         internalFilter.Apply(schema, context, metadata);
      }
      else
      {
         _adHocUnionSchemaFilter.Apply(schema, context);
      }
   }

   private void Apply(
      OpenApiSchema schema,
      SchemaFilterContext context,
      Metadata.RegularUnion metadata)
   {
      _regularUnionSchemaFilter.Apply(schema, context);
   }
}
