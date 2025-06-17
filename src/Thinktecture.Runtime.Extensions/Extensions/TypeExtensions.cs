using System.Reflection;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static IReadOnlyList<ObjectFactoryMetadata> FindObjectFactoryMetadata(this Type type)
   {
      if (!typeof(IObjectFactoryOwner).IsAssignableFrom(type))
         return [];

      var objectFactoriesProperty = type.GetProperty(
         "global::Thinktecture.Internal.IObjectFactoryOwner.ObjectFactories",
         BindingFlags.Static | BindingFlags.NonPublic);

      if (objectFactoriesProperty is not null)
      {
         return (IReadOnlyList<ObjectFactoryMetadata>?)objectFactoriesProperty.GetValue(null)
                ?? throw new InvalidOperationException($"Could not retrieve object factories for type '{type.FullName}'.");
      }

      return [];
   }
}
