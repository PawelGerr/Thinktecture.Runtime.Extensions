using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Thinktecture.Swashbuckle.Internal.SmartEnums;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class DefaultSmartEnumSchemaFilter : SmartEnumSchemaFilterBase
{
   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public DefaultSmartEnumSchemaFilter(
      IServiceProvider serviceProvider,
      IOpenApiValueFactoryProvider valueFactoryProvider,
      IOptions<ThinktectureSchemaFilterOptions> options)
      : base(serviceProvider, valueFactoryProvider, options)
   {
   }

   /// <inheritdoc />
   protected override void SetItems(OpenApiSchema schema, IReadOnlyList<SmartEnumItem> items)
   {
      schema.Enum = items
                    .Select(item => item.OpenApiValue)
                    .ToList();
   }
}
