using System.Diagnostics.CodeAnalysis;

namespace Thinktecture;

/// <summary>
/// Interface of a Smart Enum.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
public interface ISmartEnum<TKey> : IKeyedObject<TKey>
   where TKey : notnull;

/// <summary>
/// Interface of a Smart Enum.
/// </summary>
/// <typeparam name="T">Type of the enumeration implementing this interface.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
/// <remarks>
/// Don't implement this interface directly. It will be implemented by a source generator.
/// </remarks>
public interface ISmartEnum<TKey, T, out TValidationError> : ISmartEnum<TKey>, IObjectFactory<T, TKey, TValidationError>
   where T : ISmartEnum<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Gets all valid items.
   /// </summary>
   static abstract IReadOnlyList<T> Items { get; }

   /// <summary>
   /// Gets an enumeration item for provided <paramref name="key"/>.
   /// </summary>
   /// <param name="key">The identifier to return an enumeration item for.</param>
   /// <returns>An instance of <typeparamref name="T"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
   [return: NotNullIfNotNull(nameof(key))]
   static abstract T? Get(TKey? key);

   /// <summary>
   /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
   /// </summary>
   /// <param name="key">The identifier to return an enumeration item for.</param>
   /// <param name="item">An instance of <typeparamref name="T"/>.</param>
   /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
   static abstract bool TryGet(TKey? key, [MaybeNullWhen(false)] out T item);
}
