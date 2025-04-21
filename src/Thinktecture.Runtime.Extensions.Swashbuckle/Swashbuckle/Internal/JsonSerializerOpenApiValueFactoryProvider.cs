using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public class JsonSerializerOpenApiValueFactoryProvider : IOpenApiValueFactoryProvider
{
   private readonly JsonSerializerOptionsResolver _jsonSerializerOptionsResolver;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   public JsonSerializerOpenApiValueFactoryProvider(JsonSerializerOptionsResolver jsonSerializerOptionsResolver)
   {
      _jsonSerializerOptionsResolver = jsonSerializerOptionsResolver;
   }

   /// <inheritdoc />
   public bool TryGet(
      Type type,
      [MaybeNullWhen(false)] out IOpenApiValueFactory valueFactory)
   {
      valueFactory = new JsonSerializerOpenApiValueFactory(type, _jsonSerializerOptionsResolver.JsonSerializerOptions);
      return true;
   }
}
