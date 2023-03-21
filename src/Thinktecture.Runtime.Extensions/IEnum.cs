using System.Diagnostics.CodeAnalysis;

namespace Thinktecture;

/// <summary>
/// Interface of a Smart Enum.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
public interface IEnum<TKey> : IKeyedValueObject<TKey>
   where TKey : notnull
{
   /// <summary>
   /// Key equality comparer.
   /// </summary>
   static virtual IEqualityComparer<TKey> KeyEqualityComparer => throw new NotImplementedException("This property will be implemented by the source generator.");
}

/// <summary>
/// Interface of a Smart Enum.
/// </summary>
/// <typeparam name="T">Type of the enumeration implementing this interface.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <remarks>
/// Don't implement this interface directly. It will be implemented by a source generator.
/// </remarks>
public interface IEnum<TKey, T> : IKeyedValueObject<T, TKey>
   where T : IEnum<TKey>
   where TKey : notnull
{
   /// <summary>
   /// Gets all valid items.
   /// </summary>
   static virtual IReadOnlyList<T> Items => throw new NotImplementedException("This property will be implemented by the source generator.");

   /// <summary>
   /// Gets an enumeration item for provided <paramref name="key"/>.
   /// </summary>
   /// <param name="key">The identifier to return an enumeration item for.</param>
   /// <returns>An instance of <typeparamref name="T"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
   [return: NotNullIfNotNull(nameof(key))]
   static virtual T? Get(TKey? key) => throw new NotImplementedException("This method will be implemented by the source generator.");

   /// <summary>
   /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
   /// </summary>
   /// <param name="key">The identifier to return an enumeration item for.</param>
   /// <param name="item">A valid instance of <typeparamref name="T"/>; otherwise <c>null</c>.</param>
   /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
   static virtual bool TryGet(TKey? key, [MaybeNullWhen(false)] out T item) => throw new NotImplementedException("This method will be implemented by the source generator.");
}
