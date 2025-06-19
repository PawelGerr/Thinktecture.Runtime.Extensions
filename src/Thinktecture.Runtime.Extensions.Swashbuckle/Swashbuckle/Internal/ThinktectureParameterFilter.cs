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
      if (context.ParameterInfo is null)
         return;

      var modelBindingType = GetModelBindingType(context.ParameterInfo.ParameterType);

      if (modelBindingType is null)
         return;

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
