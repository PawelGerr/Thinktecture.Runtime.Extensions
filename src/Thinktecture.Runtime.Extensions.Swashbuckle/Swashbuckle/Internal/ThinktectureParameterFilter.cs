using Microsoft.Extensions.Options;
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
   private readonly bool _createExtraSchemasForParameters;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public ThinktectureParameterFilter(
      IOptions<ThinktectureSchemaFilterOptions> options)
   {
      _createExtraSchemasForParameters = options.Value.CreateExtraSchemasForParameters;
   }

   /// <inheritdoc />
   public void Apply(
      OpenApiParameter parameter,
      ParameterFilterContext context)
   {
      if (context.ParameterInfo is null)
         return;

      var modelBindingType = GetModelBindingType(context.ParameterInfo.ParameterType);

      if (modelBindingType is null)
         return;

      // Make Nullable<modelBindingType> if it is a struct and the parameter is a nullable struct as well.
      if (context.ParameterInfo.ParameterType.IsValueType && modelBindingType.IsValueType)
      {
         if (Nullable.GetUnderlyingType(context.ParameterInfo.ParameterType) is not null)
         {
            if (Nullable.GetUnderlyingType(modelBindingType) is null)
               modelBindingType = typeof(Nullable<>).MakeGenericType(modelBindingType);
         }
      }

      if (parameter.Schema.Type is null                               // Wrapper made by UseAllOfToExtendReferenceSchemas.
          && context.ParameterInfo.ParameterType == modelBindingType) // We keep the reference if the types are equal.
         return;

      // We need a new schema, something like MyComplexTypeAsString
      if (_createExtraSchemasForParameters && context.ParameterInfo.ParameterType != modelBindingType)
         modelBindingType = typeof(BoundParameter<,>).MakeGenericType(context.ParameterInfo.ParameterType, modelBindingType);

      parameter.Schema = context.SchemaGenerator.GenerateSchema(
         modelBindingType,
         context.SchemaRepository,
         parameterInfo: context.ParameterInfo);
   }

   private Type? GetModelBindingType(Type parameterType)
   {
      // 1) Object factory with UseForModelBinding has precedence over metadata
      var metadataForModelBinding = MetadataLookup.FindMetadataForConversion(
         parameterType,
         f => f.UseForModelBinding,
         _ => false);

      if (metadataForModelBinding is not null)
         return metadataForModelBinding.Value.KeyType;

      // 2) It is assumed that keyed objects are bindable by default
      if (MetadataLookup.Find(parameterType) is Metadata.Keyed keyedMetadata)
      {
         return keyedMetadata.Type;
      }

      // 3) IParsable is our last resort
      var parsableMetadata = MetadataLookup.FindMetadataForConversion(
         parameterType,
         f => f.ValueType == typeof(string),
         _ => false);

      return parsableMetadata?.KeyType;
   }
}
