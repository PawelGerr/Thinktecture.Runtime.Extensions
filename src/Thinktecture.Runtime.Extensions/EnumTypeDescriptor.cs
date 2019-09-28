using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Thinktecture
{
   /// <summary>
   /// Type descriptor for <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   public class EnumTypeDescriptor : CustomTypeDescriptor
   {
      private static readonly ConcurrentDictionary<Type, TypeConverter> _converterLookup = new ConcurrentDictionary<Type, TypeConverter>();

      private readonly Type _objectType;

      /// <summary>
      /// Initializes new instance of <see cref="EnumTypeDescriptor"/>.
      /// </summary>
      /// <param name="parent">Parent type descriptor.</param>
      /// <param name="objectType">Type of an enumeration.</param>
      public EnumTypeDescriptor(ICustomTypeDescriptor parent, Type objectType)
         : base(parent)
      {
         _objectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
      }

      /// <inheritdoc />
      public override TypeConverter GetConverter()
      {
         return GetCachedConverter(_objectType);
      }

      private static TypeConverter GetCachedConverter(Type type)
      {
         return _converterLookup.GetOrAdd(type, CreateTypeConverter);
      }

      private static TypeConverter CreateTypeConverter(Type type)
      {
         var enumTypes = GetEnumTypesArguments(type);
         var converterType = typeof(EnumTypeConverter<,>).MakeGenericType(enumTypes);

         return (TypeConverter)Activator.CreateInstance(converterType);
      }

      private static Type[] GetEnumTypesArguments(Type type)
      {
         var typeDef = type.FindGenericEnumTypeDefinition();

         if (typeDef != null)
            return typeDef.GenericTypeArguments;

         throw new ArgumentException($"The provided type {type.FullName} does not inherit the type Enum<,>");
      }
   }
}
