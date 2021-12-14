using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Internal;

/// <summary>
/// Tries to convert the value of type <typeparamref name="TKey"/> to an instance of type <typeparamref name="T"/>.
/// </summary>
/// <param name="key">Value to convert.</param>
/// <param name="model">Converted <paramref name="key"/>.</param>
/// <typeparam name="T">Type the <paramref name="key"/> to convert to.</typeparam>
/// <typeparam name="TKey">Type of the <typeparamref name="TKey"/>.</typeparam>
public delegate ValidationResult? Validate<T, in TKey>(TKey key, out T? model)
   where TKey : notnull;
