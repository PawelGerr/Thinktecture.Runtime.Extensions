using System;
using System.Reflection;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extension methods for <see cref="Type"/>.
   /// </summary>
   public static class TypeExtensions
   {
      /// <summary>
      /// Searches for the (base) type implementing <see cref="Enum{TEnum,TKey}"/>
      /// </summary>
      /// <param name="type">Type to search in.</param>
      /// <returns>A generic type created from generic <see cref="Enum{TEnum,TKey}"/> or <c>null</c> if the <paramref name="type"/> is not a generic enum.</returns>
      /// <exception cref="ArgumentNullException">Type is <c>null</c>.</exception>
      [CanBeNull]
      public static TypeInfo FindGenericEnumTypeDefinition([NotNull] this Type type)
      {
         if (type == null)
            throw new ArgumentNullException(nameof(type));

         while (type != typeof(object))
         {
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType)
            {
               var genericType = typeInfo.GetGenericTypeDefinition();

               if (genericType == typeof(Enum<,>))
                  return typeInfo;
            }

            type = typeInfo.BaseType;
         }

         return null;
      }
   }
}
