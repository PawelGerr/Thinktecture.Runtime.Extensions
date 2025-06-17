using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal.RegularUnions;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class RegularUnionSchemaFilter : IRegularUnionSchemaFilter
{
   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context)
   {
   }
}
