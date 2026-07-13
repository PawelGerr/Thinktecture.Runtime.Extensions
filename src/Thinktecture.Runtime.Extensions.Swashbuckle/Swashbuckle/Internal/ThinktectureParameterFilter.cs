using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
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
      IOpenApiParameter parameter,
      ParameterFilterContext context)
   {
      if (context.ParameterInfo is null)
         return;

      var modelBindingType = GetModelBindingType(context.ParameterInfo.ParameterType);

      if (modelBindingType is null)
         return;

      modelBindingType = context.ParameterInfo.ParameterType.NormalizeStructType(modelBindingType);
      var serializationType = ThinktectureSchemaFilter.GetSerializationType(context.ParameterInfo.ParameterType);

      if (parameter is not OpenApiParameter openApiParameter
          || (parameter.Schema?.AllOf?.Count > 0         // Wrapper made by UseAllOfToExtendReferenceSchemas.
              && modelBindingType == serializationType)) // We keep the reference if the types are equal.
         return;

      // We need a new schema, something like MyComplexTypeAsString
      if (_createExtraSchemasForParameters && context.ParameterInfo.ParameterType != modelBindingType)
         modelBindingType = typeof(BoundParameter<,>).MakeGenericType(context.ParameterInfo.ParameterType, modelBindingType);

      openApiParameter.Schema = context.SchemaGenerator.GenerateSchema(
         modelBindingType,
         context.SchemaRepository,
         parameterInfo: context.ParameterInfo);
   }

   private Type? GetModelBindingType(Type parameterType)
   {
      // 1) Object factory with UseForModelBinding has precedence over metadata. This mirrors
      // ThinktectureModelBinderProvider, which also excludes ReadOnlySpan<char>-based factories because a ref
      // struct cannot be used as the generic key argument of the model binder.
      var metadataForModelBinding = MetadataLookup.FindMetadataForConversion(
         parameterType,
         f => f.ValueType != typeof(ReadOnlySpan<char>) && f.UseForModelBinding,
         _ => false);

      if (metadataForModelBinding is not null)
         return metadataForModelBinding.Value.KeyType;

      // 2) It is assumed that keyed objects are bindable by default via their key. The type's own schema normally
      // represents that key (e.g. a Smart Enum documents its allowed key values), so returning the type keeps the
      // richer schema. But when a serialization-only object factory makes the type serialize as a different wire
      // type, the type's own schema would document that serialization format instead of the key. In that case
      // return the key type, because ThinktectureModelBinderProvider falls back to the key for model binding.
      if (MetadataLookup.Find(parameterType) is Metadata.Keyed keyedMetadata)
      {
         return ThinktectureSchemaFilter.GetSerializationType(parameterType) == parameterType
                   ? keyedMetadata.Type
                   : keyedMetadata.KeyType;
      }

      // 3) IParsable is our last resort
      var parsableMetadata = MetadataLookup.FindMetadataForConversion(
         parameterType,
         f => f.ValueType == typeof(string),
         _ => false);

      return parsableMetadata?.KeyType;
   }
}
