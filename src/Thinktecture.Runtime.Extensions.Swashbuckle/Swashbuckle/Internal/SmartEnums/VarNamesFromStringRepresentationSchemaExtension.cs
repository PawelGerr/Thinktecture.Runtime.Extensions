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
public class VarNamesFromStringRepresentationSchemaExtension : ISmartEnumSchemaExtension
{
   private readonly ILogger<VarNamesFromStringRepresentationSchemaExtension> _logger;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public VarNamesFromStringRepresentationSchemaExtension(
      ILogger<VarNamesFromStringRepresentationSchemaExtension> logger)
   {
      _logger = logger;
   }

   /// <inheritdoc />
   public void Apply(OpenApiSchema schema, SchemaFilterContext context, IEnumerable<object> items)
   {
      var duplicates = items.Select(i => i.ToString())
                            .GroupBy(n => n, StringComparer.Ordinal)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

      if (duplicates.Count > 0)
      {
         _logger.LogWarning("The string representation of enum items of '{Type}' contains duplicates. The 'x-enum-varnames' extension will not be set. Duplicates: {Duplicates}.",
                            context.Type.FullName,
                            String.Join(", ", duplicates.Select(d => d)));

         return;
      }

      var names = new OpenApiArray();
      names.AddRange(items.Select(item => new OpenApiString(item.ToString())));

      schema.Extensions["x-enum-varnames"] = names;
   }
}
