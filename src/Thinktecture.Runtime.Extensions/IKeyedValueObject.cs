using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Thinktecture;

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
public interface IKeyedValueObject
{
}

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public interface IKeyedValueObject<out TKey> : IKeyedValueObject
   where TKey : notnull
{
   /// <summary>
   /// Gets the key of the item.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   TKey GetKey() => throw new NotImplementedException("This method will be implemented by the source generator.");
}

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <remarks>
/// Don't implement this interface directly. It will be implemented by a source generator.
/// </remarks>
public interface IKeyedValueObject<T, TKey> : IKeyedValueObject<TKey>
   where TKey : notnull
{
   /// <summary>
   /// Validate the <paramref name="key"/> and returns an <paramref name="item"/> if the validation succeeded.
   /// </summary>
   /// <param name="key">The value to validate.</param>
   /// <param name="item">Item with key property equals to the provided <paramref name="key"/>.</param>
   /// <returns>Validation result.</returns>
   static virtual ValidationResult? Validate(TKey? key, out T? item) => throw new NotImplementedException("This method will be implemented by the source generator.");
}
