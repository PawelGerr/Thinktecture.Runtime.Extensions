using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class ThinktectureParameterFilter : IParameterFilter
{
   /// <inheritdoc />
   public void Apply(
      OpenApiParameter parameter,
      ParameterFilterContext context)
   {
      var metadata = context.ParameterInfo is null
                        ? null
                        : MetadataLookup.Find(context.ParameterInfo.ParameterType);

      metadata?.Switch(
         (Filter: this, parameter, context),
         keylessSmartEnum: static (state, smartEnumMetadata) => Apply(state.parameter, state.context, smartEnumMetadata),
         keyedSmartEnum: static (state, smartEnumMetadata) => Apply(state.parameter, state.context, smartEnumMetadata),
         keyedValueObject: static (state, keyedValueObjectMetadata) => Apply(state.parameter, state.context, keyedValueObjectMetadata),
         complexValueObject: static (state, complexValueObjectMetadata) => Apply(state.parameter, state.context, complexValueObjectMetadata),
         adHocUnion: static (state, adHocUnionMetadata) => Apply(state.parameter, state.context, adHocUnionMetadata),
         regularUnion: static (state, regularUnionMetadata) => Apply(state.parameter, state.context, regularUnionMetadata));
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.KeylessSmartEnum metadata)
   {
      // IParsable
      if (metadata.ObjectFactories.Any(f => f.ValueType == typeof(string)))
      {
         parameter.Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository, parameterInfo: context.ParameterInfo);
      }
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.Keyed.SmartEnum metadata)
   {
      parameter.Schema = context.SchemaGenerator.GenerateSchema(metadata.Type, context.SchemaRepository);
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.Keyed.ValueObject metadata)
   {
      parameter.Schema = context.SchemaGenerator.GenerateSchema(metadata.Type, context.SchemaRepository);
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.ComplexValueObject metadata)
   {
      // IParsable
      if (metadata.ObjectFactories.Any(f => f.ValueType == typeof(string)))
      {
         parameter.Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository, parameterInfo: context.ParameterInfo);
      }
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.AdHocUnion metadata)
   {
      // IParsable
      if (metadata.ObjectFactories.Any(f => f.ValueType == typeof(string)))
      {
         parameter.Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository, parameterInfo: context.ParameterInfo);
      }
   }

   private static void Apply(OpenApiParameter parameter, ParameterFilterContext context, Metadata.RegularUnion metadata)
   {
      // IParsable
      if (metadata.ObjectFactories.Any(f => f.ValueType == typeof(string)))
      {
         parameter.Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository, parameterInfo: context.ParameterInfo);
      }
   }
}
