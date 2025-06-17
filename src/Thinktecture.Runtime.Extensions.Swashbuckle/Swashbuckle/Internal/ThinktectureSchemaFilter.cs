using Microsoft.Extensions.Options;
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
