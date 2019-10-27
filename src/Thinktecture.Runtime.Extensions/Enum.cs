using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
   [SuppressMessage("ReSharper", "CA1711")]
   [SuppressMessage("ReSharper", "CA1716")]
   public abstract class Enum<TEnum> : Enum<TEnum, string>
      where TEnum : Enum<TEnum, string>
   {
      static Enum()
      {
         KeyEqualityComparer = StringComparer.OrdinalIgnoreCase;
      }

      /// <summary>
      /// Initializes new instance of <see cref="Enum{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="key">The key of the enumeration item.</param>
      protected Enum(string key)
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
   [SuppressMessage("ReSharper", "CA1711")]
   [SuppressMessage("ReSharper", "CA1716")]
   [SuppressMessage("ReSharper", "CA1000")]
   [SuppressMessage("ReSharper", "CA2201")]
   [TypeDescriptionProvider(typeof(EnumTypeDescriptionProvider))]
   public abstract class Enum<TEnum, TKey> : IEquatable<Enum<TEnum, TKey>?>, IEnum
      where TEnum : Enum<TEnum, TKey>
      where TKey : notnull
   {
      private static IEqualityComparer<TKey>? _keyEqualityComparer;

      /// <summary>
      /// Equality comparer for keys. Default is <see cref="EqualityComparer{TKey}.Default"/>.
      /// Important: This property may be changed once and in static constructor only!
      /// </summary>
      protected static IEqualityComparer<TKey> KeyEqualityComparer
      {
         get => _keyEqualityComparer ?? _defaultKeyEqualityComparer;
         set
         {
            if (_items?.Count > 0)
               throw new InvalidOperationException($"Setting the {KeyEqualityComparer} must be done in static constructor of {typeof(TEnum).FullName}.");

            _keyEqualityComparer = value;
         }
      }

      private static readonly EqualityComparer<TKey> _defaultKeyEqualityComparer = EqualityComparer<TKey>.Default;
      private static readonly Func<TKey, TEnum> _invalidEnumFactory = GetInvalidEnumerationFactory();
      private static readonly int _typeHashCode = typeof(TEnum).GetHashCode() * 397;

      // do not initialize items in static ctor
      // because the static fields of the derived class may not be initialized yet.
      private static Dictionary<TKey, TEnum>? _itemsLookup;
      private static IReadOnlyList<TEnum>? _items;

      private static Dictionary<TKey, TEnum> ItemsLookup => _itemsLookup ??= GetItems();
      private static IReadOnlyList<TEnum> Items => _items ??= ItemsLookup.Values.ToList().AsReadOnly();

      private static Func<TKey, TEnum> GetInvalidEnumerationFactory()
      {
         var type = typeof(TEnum);
         var method = type.GetTypeInfo().GetMethod(nameof(CreateInvalid), BindingFlags.Instance | BindingFlags.NonPublic);

         if (method == null)
            throw new Exception($"The method {nameof(CreateInvalid)} hasn't been found in enumeration of type {type.FullName}.");

         return (Func<TKey, TEnum>)method.CreateDelegate(typeof(Func<TKey, TEnum>), null);
      }

      private static Dictionary<TKey, TEnum> GetItems()
      {
         var type = typeof(TEnum);
         var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

         // access to static field triggers the static constructors of derived classes
         // so the class gets initialized properly and we get a valid KeyEqualityComparer
         if (fields.Any())
            fields.First().GetValue(null);

         var items = fields.Where(f => f.FieldType == type)
                           .Select(f =>
                                   {
                                      if (!f.IsInitOnly)
                                         throw new Exception($"The field \"{f.Name}\" of enumeration type \"{type.FullName}\" must be read-only.");

                                      var item = (TEnum)f.GetValue(null);

                                      if (item is null)
                                         throw new Exception($"The field \"{f.Name}\" of enumeration type \"{type.FullName}\" is not initialized.");

                                      if (!item.IsValid)
                                         throw new Exception($"The field \"{f.Name}\" of enumeration type \"{type.FullName}\" is not valid.");

                                      return item;
                                   });

         var lookup = new Dictionary<TKey, TEnum>(KeyEqualityComparer);

         foreach (var item in items)
         {
            if (lookup.ContainsKey(item.Key))
               throw new ArgumentException($"The enumeration of type \"{type.FullName}\" has multiple items with the key \"{item.Key}\".");

            lookup.Add(item.Key, item);
         }

         return lookup;
      }

      /// <inheritdoc />
      object IEnum.Key => Key;

      /// <summary>
      /// The key of the enumeration item.
      /// </summary>
      [NotNull]
      public TKey Key { get; }

      /// <inheritdoc />
      public bool IsValid { get; private set; }

      private readonly int _hashCode;
      private readonly string _toString;

      /// <summary>
      /// Initializes new valid instance of <see cref="Enum{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="key">The key of the enumeration item.</param>
      protected Enum([NotNull] TKey key)
      {
         if (key is null)
            throw new ArgumentNullException(nameof(key));

         Key = key;
         IsValid = true;

         _hashCode = _typeHashCode ^ KeyEqualityComparer.GetHashCode(Key);
         _toString = Key.ToString();
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
      /// <returns>A collection with all valid items.</returns>
      [SuppressMessage("ReSharper", "CA1024")]
      public static IReadOnlyList<TEnum> GetAll()
      {
         return Items;
      }

      /// <summary>
      /// Gets an enumeration item for provided <paramref name="key"/>.
      /// </summary>
      /// <param name="key">The key to return an enumeration item for.</param>
      /// <returns>An instance of <typeparamref name="TEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      [return: NotNullIfNotNull("key")]
      public static TEnum? Get([AllowNull] TKey key)
      {
         if (key is null)
            return null;

         if (!ItemsLookup.TryGetValue(key, out var item))
         {
            item = _invalidEnumFactory(key);

            if (item is null)
               throw new Exception($"The method {nameof(CreateInvalid)} of enumeration type {typeof(TEnum).FullName} returned null.");

            item.IsValid = false;
         }

         return item;
      }

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
      /// </summary>
      /// <param name="key">The key to return an enumeration item for.</param>
      /// <param name="item">A valid instance of <typeparamref name="TEnum"/>; otherwise <c>null</c>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([AllowNull] TKey key, [NotNullWhen(true)] out TEnum? item)
      {
         if (key is null)
         {
            item = default;
            return false;
         }

         return ItemsLookup.TryGetValue(key, out item);
      }

      /// <inheritdoc />
      public void EnsureValid()
      {
         if (!IsValid)
            throw new InvalidOperationException($"The current enumeration item of type {typeof(TEnum).FullName} with key {Key} is not valid.");
      }

      /// <inheritdoc />
      public bool Equals(Enum<TEnum, TKey>? other)
      {
         if (other is null)
            return false;
         if (!ReferenceEquals(GetType(), other.GetType()))
            return false;
         if (ReferenceEquals(this, other))
            return true;

         // valid items are "ReferenceEquals" so if we are here
         // then one of the items are valid the other is not.
         if (IsValid || other.IsValid)
            return false;

         return KeyEqualityComparer.Equals(Key, other.Key);
      }

      /// <inheritdoc />
      public override bool Equals(object? otherEnum)
      {
         return Equals(otherEnum as Enum<TEnum, TKey>);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return _hashCode;
      }

      /// <inheritdoc />
      public override string ToString()
      {
         return _toString;
      }

      /// <summary>
      /// Implicit conversion to the type of <typeparamref name="TKey"/>.
      /// </summary>
      /// <param name="item">Item to covert.</param>
      /// <returns>The <see cref="Key"/> of provided <paramref name="item"/> or <c>null</c> if a<paramref name="item"/> is <c>null</c>.</returns>
      [SuppressMessage("ReSharper", "CA2225")]
      [return: NotNullIfNotNull("item")]
      public static implicit operator TKey(Enum<TEnum, TKey>? item)
      {
         return item == null ? default : item.Key;
      }

      /// <summary>
      /// Compares to instances of <see cref="Enum{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="item1">Instance to compare.</param>
      /// <param name="item2">Another instance to compare.</param>
      /// <returns><c>true</c> if items are equal; otherwise <c>false</c>.</returns>
      public static bool operator ==(Enum<TEnum, TKey>? item1, Enum<TEnum, TKey>? item2)
      {
         if (item1 is null)
            return item2 is null;

         return item1.Equals(item2);
      }

      /// <summary>
      /// Compares to instances of <see cref="Enum{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="item1">Instance to compare.</param>
      /// <param name="item2">Another instance to compare.</param>
      /// <returns><c>false</c> if items are equal; otherwise <c>true</c>.</returns>
      public static bool operator !=(Enum<TEnum, TKey>? item1, Enum<TEnum, TKey>? item2)
      {
         return !(item1 == item2);
      }
   }
}
