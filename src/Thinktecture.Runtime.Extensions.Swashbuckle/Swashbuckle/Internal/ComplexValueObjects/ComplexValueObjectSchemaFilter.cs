using Microsoft.OpenApi.Models;
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

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public ComplexValueObjectSchemaFilter(IRequiredMemberEvaluator requiredMemberEvaluator)
   {
      _requiredMemberEvaluator = requiredMemberEvaluator;
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
         if (_requiredMemberEvaluator.IsRequired(schema, context, memberInfo))
            schema.Required.Add(memberInfo.Name);
      }
   }
}
