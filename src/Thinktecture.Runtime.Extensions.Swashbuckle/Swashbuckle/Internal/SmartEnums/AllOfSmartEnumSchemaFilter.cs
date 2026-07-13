using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Thinktecture.Swashbuckle.Internal.SmartEnums;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class AllOfSmartEnumSchemaFilter : SmartEnumSchemaFilterBase
{
   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public AllOfSmartEnumSchemaFilter(
      IServiceProvider serviceProvider,
      IOpenApiValueFactoryProvider valueFactoryProvider,
      IOptions<ThinktectureSchemaFilterOptions> options)
      : base(serviceProvider, valueFactoryProvider, options)
   {
   }

   /// <inheritdoc />
   protected override void SetItems(OpenApiSchema schema, IReadOnlyList<SmartEnumItem> items)
   {
      // "allOf" is a conjunction: a value must satisfy every subschema at once. Emitting one
      // single-value subschema per item would require the value to equal all items simultaneously,
      // which no value can. The items are therefore combined into a single "enum" subschema (as in
      // "DefaultSmartEnumSchemaFilter"), appended to any pre-existing "allOf" entries, for example a
      // "$ref" to the key type.
      var enumSchema = new OpenApiSchema
                       {
                          Enum = items.Select(item => item.OpenApiValue).ToList()
                       };

      schema.AllOf =
      [
         .. schema.AllOf ?? Array.Empty<OpenApiSchema>(),
         enumSchema
      ];
   }
}
