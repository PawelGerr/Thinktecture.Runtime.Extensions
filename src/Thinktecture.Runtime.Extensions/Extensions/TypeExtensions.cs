using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extension methods for <see cref="Type"/>.
   /// </summary>
   public static class TypeExtensions
   {
      /// <summary>
      /// Searches for the (base) type implementing <see cref="IEnum{TKey}"/>
      /// </summary>
      /// <param name="type">Type to search in.</param>
      /// <returns>A generic type created from generic <see cref="IEnum{TKey}"/> or <c>null</c> if the <paramref name="type"/> is not a generic enum.</returns>
      /// <exception cref="ArgumentNullException">Type is <c>null</c>.</exception>
      public static TypeInfo? FindGenericEnumTypeDefinition(this Type? type)
      {
         if (type is null)
            return null;

         var interfaces = type.GetTypeInfo().ImplementedInterfaces;

         foreach (var baseType in interfaces)
         {
            if (baseType.IsGenericType)
            {
               var genericType = baseType.GetGenericTypeDefinition();

               if (genericType == typeof(IEnum<>))
                  return baseType.GetTypeInfo();
            }
         }

         return null;
      }
   }
}
