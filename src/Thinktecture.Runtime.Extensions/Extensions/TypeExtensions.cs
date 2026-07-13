using System.Reflection;
using Thinktecture.Internal;

namespace Thinktecture;

internal static class TypeExtensions
{
   public static (Type? OwningType, IReadOnlyList<ObjectFactoryMetadata> Factories) FindObjectFactoryMetadata(this Type type)
   {
      if (!typeof(IObjectFactoryOwner).IsAssignableFrom(type))
         return (null, []);

      // The generated ObjectFactories property is a private static explicit interface implementation on the annotated
      // type. Reflection never returns private static members of a base type, so the base types are walked explicitly,
      // mirroring MetadataLookup.SearchBaseTypesForMetadata. Without this, a derived class of a standalone [ObjectFactory]
      // type loses its object factories.
      //
      // The type that actually declares the property is returned as well, because that is the type the object factories
      // (and their generated IObjectFactory<T, ...> interface) are created for. Consumers must build conversion metadata
      // from this owning type, not from the (possibly derived) requested type, otherwise their MakeGenericType calls
      // violate the invariant IObjectFactory<T, ...> constraint.
      var typeToCheck = type;

      while (typeToCheck is not null)
      {
         var objectFactoriesProperty = typeToCheck.GetProperty(
            "global::Thinktecture.Internal.IObjectFactoryOwner.ObjectFactories",
            BindingFlags.Static | BindingFlags.NonPublic);

         if (objectFactoriesProperty is not null)
         {
            var factories = (IReadOnlyList<ObjectFactoryMetadata>?)objectFactoriesProperty.GetValue(null)
                            ?? throw new InvalidOperationException($"Could not retrieve object factories for type '{type.FullName}'.");

            return (typeToCheck, factories);
         }

         typeToCheck = typeToCheck.BaseType;
      }

      return (null, []);
   }
}
