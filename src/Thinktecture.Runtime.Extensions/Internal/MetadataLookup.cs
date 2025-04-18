using System.Collections.Concurrent;
using System.Reflection;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public static class MetadataLookup
{
   private static readonly ConcurrentDictionary<Type, Metadata> _metadata = new();

   /// <summary>
   /// Searches for <see cref="Metadata"/> for provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search <see cref="Metadata"/> for.</param>
   /// <returns>An instance of <see cref="Metadata"/> if the <paramref name="type"/> has metadata; otherwise <c>null</c>.</returns>
   public static Metadata? Find(Type? type)
   {
      if (type is null)
         return null;

      type = type.IsClass ? type : Nullable.GetUnderlyingType(type) ?? type;

      if (_metadata.TryGetValue(type, out var metadata))
         return metadata;

      if (!typeof(IMetadataOwner).IsAssignableFrom(type))
         return null;

      metadata = SearchBaseTypesForMetadata(type);
      _metadata.TryAdd(type, metadata);

      return metadata;
   }

   private static Metadata SearchBaseTypesForMetadata(Type type)
   {
      var typeToCheck = type;

      while (typeToCheck is not null)
      {
         if (_metadata.TryGetValue(typeToCheck, out var metadata))
            return metadata;

         var property = typeToCheck.GetProperty(
            "global::Thinktecture.Internal.IMetadataOwner.Metadata",
            BindingFlags.Static | BindingFlags.NonPublic);

         if (property is not null)
         {
            return (Metadata?)property.GetValue(null)
                   ?? throw new InvalidOperationException($"Could not retrieve metadata for type '{type.FullName}'.");
         }

         typeToCheck = typeToCheck.BaseType;
      }

      throw new Exception($"Could not retries metadata for type '{type.FullName}'.");
   }
}
