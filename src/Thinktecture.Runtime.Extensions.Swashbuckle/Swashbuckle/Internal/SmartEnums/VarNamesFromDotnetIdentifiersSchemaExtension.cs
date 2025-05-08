using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal.SmartEnums;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class VarNamesFromDotnetIdentifiersSchemaExtension : ISmartEnumSchemaExtension
{
   private readonly ILogger<VarNamesFromStringRepresentationSchemaExtension> _logger;
   private readonly string _extensionName;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public VarNamesFromDotnetIdentifiersSchemaExtension(
      ILogger<VarNamesFromStringRepresentationSchemaExtension> logger,
      string extensionName = "x-enum-varnames")
   {
      _logger = logger;
      _extensionName = extensionName;
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, IReadOnlyList<ISmartEnumItem> items)
   {
      var names = new OpenApiArray();
      names.AddRange(items.Select(item => new OpenApiString(item.Identifier.ToString())));

      schema.Extensions[_extensionName] = names;
   }
}
