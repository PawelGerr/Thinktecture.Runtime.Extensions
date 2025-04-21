using System.Text.Json;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class JsonSerializerOpenApiValueFactory : IOpenApiValueFactory
{
   private readonly Type _type;
   private readonly JsonSerializerOptions _jsonSerializerOptions;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public JsonSerializerOpenApiValueFactory(
      Type type,
      JsonSerializerOptions jsonSerializerOptions)
   {
      _type = type;
      _jsonSerializerOptions = jsonSerializerOptions;
   }

   /// <inheritdoc />
   public IOpenApiAny CreateOpenApiValue(object value)
   {
      var json = JsonSerializer.Serialize(value, _type, _jsonSerializerOptions);
      return OpenApiAnyFactory.CreateFromJson(json, _jsonSerializerOptions);
   }
}
