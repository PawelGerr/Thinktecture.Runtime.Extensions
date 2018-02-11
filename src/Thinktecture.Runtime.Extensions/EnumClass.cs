using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Thinktecture
{
	/// <summary>
	/// Base class for enum-like classes using a <see cref="string"/> as a key.
	/// </summary>
	/// <remarks>
	/// Derived classes must have a default constructor for creation of "invalid" enumeration items.
	/// The default constructor should not be public.
	/// </remarks>
	/// <typeparam name="TEnum">Concrete type of the enumeration.</typeparam>
	public abstract class EnumClass<TEnum> : EnumClass<TEnum, string>
		where TEnum : EnumClass<TEnum, string>
	{
		static EnumClass()
		{
			KeyEqualityComparer = StringComparer.OrdinalIgnoreCase;
		}

		/// <summary>
		/// Initializes new instance of <see cref="EnumClass{TEnum,TKey}"/>.
		/// </summary>
		/// <param name="key">The key of the enumeration item.</param>
		protected EnumClass(string key)
			: base(key)
		{
		}
	}

	/// <summary>
	/// Base class for enum-like classes.
	/// </summary>
	/// <remarks>
	/// Derived classes must have a default constructor for creation of "invalid" enumeration items. 
	/// The default constructor should not be public.
	/// </remarks>
	/// <typeparam name="TEnum">Concrete type of the enumeration.</typeparam>
	/// <typeparam name="TKey">Type of the key.</typeparam>
	public abstract class EnumClass<TEnum, TKey> : IEquatable<EnumClass<TEnum, TKey>>
		where TEnum : EnumClass<TEnum, TKey>
	{
		private static IEqualityComparer<TKey> _keyEqualityComparer;

		/// <summary>
		/// Equality comparer for keys. Default is <see cref="EqualityComparer{TKey}.Default"/>.
		/// Important: This property may be changed once and in static constructor only!
		/// </summary>
		protected static IEqualityComparer<TKey> KeyEqualityComparer
		{
			get => _keyEqualityComparer ?? _defaultKeyEqualityComparer;
			set => _keyEqualityComparer = value;
		}

		private static readonly EqualityComparer<TKey> _defaultKeyEqualityComparer;
		private static readonly Func<TKey, TEnum> _invalidEnumFactory;

		// do not initialize items in static ctor
		// because the static fields of the derived class may not be initialized yet.
		private static List<TEnum> _items;
		private static List<TEnum> Items => _items ?? (_items = GetItems());

		// because this value should not be shared among instances of different close constructed types.
		// ReSharper disable once StaticMemberInGenericType
		private static readonly int _typeHashCode;

		static EnumClass()
		{
			TypeDescriptor.AddAttributes(typeof(TEnum), new TypeConverterAttribute(typeof(EnumClassTypeConverter<TEnum, TKey>)));

			_defaultKeyEqualityComparer = EqualityComparer<TKey>.Default;
			_typeHashCode = typeof(TEnum).GetHashCode() * 397;
			_invalidEnumFactory = GetInvalidEnumerationFactory();
		}

		private static Func<TKey, TEnum> GetInvalidEnumerationFactory()
		{
			var method = typeof(TEnum).GetTypeInfo().GetMethod(nameof(CreateInvalid), BindingFlags.Instance | BindingFlags.NonPublic);

			if (method == null)
				throw new Exception($"The method {nameof(CreateInvalid)} hasn't been found in enumeration of type {typeof(TEnum).FullName}.");

			return (Func<TKey, TEnum>)method.CreateDelegate(typeof(Func<TKey, TEnum>), null);
		}

		private static List<TEnum> GetItems()
		{
			var type = typeof(TEnum);
			var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

			return fields.Where(f => f.FieldType == typeof(TEnum))
						.Select(f =>
						{
							var item = (TEnum)f.GetValue(null);

							if (item == null)
								throw new Exception($"The field \"{f.Name}\" of enumeration type \"{type.FullName}\" is not initialized.");

							return item;
						})
						.ToList();
		}

		/// <summary>
		/// The key of the enumeration item.
		/// </summary>
		public TKey Key { get; }

		/// <summary>
		/// Indication whether the current enumeration item is valid or not.
		/// </summary>
		public bool IsValid { get; private set; }

		/// <summary>
		/// Initializes new valid instance of <see cref="EnumClass{TEnum,TKey}"/>.
		/// </summary>
		/// <param name="key">The key of the enumeration item.</param>
		protected EnumClass(TKey key)
		{
			Key = key;
			IsValid = true;
		}

		/// <summary>
		/// Creates an invalid instance of type <typeparamref name="TEnum"/>.
		/// </summary>
		/// <remarks>
		/// The code must be deterministic and must not access the qualifier <c>this</c> or any members of it.
		/// The returned instance must not be <c>null</c>.
		/// </remarks>
		/// <param name="key">Key of invalid item.</param>
		/// <returns>An invalid enumeration item.</returns>
		protected abstract TEnum CreateInvalid(TKey key);

		/// <summary>
		/// Gets all valid items.
		/// </summary>
		/// <returns></returns>
		public static List<TEnum> GetAll()
		{
			return Items.ToList();
		}

		/// <summary>
		/// Gets an enumeration item for provided <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The key to return an enumeration item for.</param>
		/// <returns>An instance of <typeparamref name="TEnum"/>.</returns>
		public static TEnum Get(TKey key)
		{
			var item = Items.Find(e => KeyEqualityComparer.Equals(e.Key, key));

			if (item == null)
			{
				item = _invalidEnumFactory(key);

				if (item == null)
					throw new Exception($"The method {nameof(CreateInvalid)} of enumeration type {typeof(TEnum).FullName} returned null.");

				item.IsValid = false;
			}

			return item;
		}

		/// <inheritdoc />
		public bool Equals(EnumClass<TEnum, TKey> other)
		{
			if (other == null)
				return false;
			if (!ReferenceEquals(GetType(), other.GetType()))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			if (IsValid || other.IsValid)
				return false;

			return KeyEqualityComparer.Equals(Key, other.Key);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return Equals(obj as EnumClass<TEnum, TKey>);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return _typeHashCode ^ KeyEqualityComparer.GetHashCode(Key);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Key?.ToString() ?? String.Empty;
		}
	}
}
