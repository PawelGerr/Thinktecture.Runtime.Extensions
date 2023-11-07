using System.ComponentModel.DataAnnotations;

namespace Thinktecture;

/// <summary>
/// Marker interface.
/// </summary>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
// ReSharper disable once UnusedTypeParameter
public interface IValueObjectFactory<TValue>
   where TValue : notnull
// ReSharper disable once RedundantTypeDeclarationBody
{
}

/// <summary>
/// A factory for creation of instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValue">Type of the value to create the item from.</typeparam>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
public interface IValueObjectFactory<T, TValue> : IValueObjectFactory<TValue>
   where TValue : notnull
{
   /// <summary>
   /// Validates the <paramref name="value"/> and returns an <paramref name="item"/> if the validation succeeded.
   /// </summary>
   /// <param name="value">The value to validate.</param>
   /// <param name="provider">An object that provides culture-specific formatting information.</param>
   /// <param name="item">Item with key property equals to the provided <paramref name="value"/>.</param>
   /// <returns>Validation result.</returns>
   static abstract ValidationResult? Validate(TValue? value, IFormatProvider? provider, out T? item);
}
