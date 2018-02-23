using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

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
		public EnumTypeDescriptor(ICustomTypeDescriptor parent, [NotNull] Type objectType)
			: base(parent)
		{
			_objectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
		}

		/// <inheritdoc />
		public override TypeConverter GetConverter()
		{
			return GetCachedConverter(_objectType);
		}

		private TypeConverter GetCachedConverter(Type type)
		{
			return _converterLookup.GetOrAdd(type, CreateTypeConverter);
		}

		private static TypeConverter CreateTypeConverter(Type type)
		{
			var enumTypes = GetEnumTypesArguments(type);
			var converterType = typeof(EnumTypeConverter<,>).MakeGenericType(enumTypes);

			return (TypeConverter)Activator.CreateInstance(converterType);
		}

		private static Type[] GetEnumTypesArguments([NotNull] Type type)
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
						return typeInfo.GenericTypeArguments;
				}

				type = typeInfo.BaseType;
			}

			throw new ArgumentException($"The provided type {type.FullName} does not inherit the type Enum<,>");
		}
	}
}
