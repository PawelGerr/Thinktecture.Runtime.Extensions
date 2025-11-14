using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Thinktecture.Internal;

namespace Thinktecture.Swashbuckle.Internal.ComplexValueObjects;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class ComplexValueObjectSchemaFilter : IInternalComplexValueObjectSchemaFilter
{
   private readonly IRequiredMemberEvaluator _requiredMemberEvaluator;
   private readonly JsonSerializerOptions _jsonSerializerOptions;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public ComplexValueObjectSchemaFilter(
      IServiceProvider serviceProvider,
      JsonSerializerOptionsResolver jsonSerializerOptionsResolver,
      IOptions<ThinktectureSchemaFilterOptions> options)
   {
      _requiredMemberEvaluator = options.Value.RequiredMemberEvaluator.CreateEvaluator(serviceProvider);
      _jsonSerializerOptions = jsonSerializerOptionsResolver.JsonSerializerOptions;
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
      if (MetadataLookup.Find(context.Type) is not Metadata.ComplexValueObject metadata)
         throw new InvalidOperationException($"The type '{context.Type.FullName}' is not a complex Value Object.");

      Apply(schema, context, metadata);
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, Metadata.ComplexValueObject metadata)
   {
      foreach (var memberInfo in metadata.AssignableMembers)
      {
         if (memberInfo.GetCustomAttribute<RequiredAttribute>() is not null
             || !_requiredMemberEvaluator.IsRequired(schema, context, memberInfo))
            continue;

         var name = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                    ?? _jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(memberInfo.Name)
                    ?? memberInfo.Name;

         schema.Required ??= new SortedSet<string>();
         schema.Required.Add(name);
      }
   }
}
