using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Internal;

/// <summary>
/// Get a keyed object with specified <paramref name="key"/>.
/// </summary>
/// <param name="key">The key to get the object for.</param>
/// <param name="obj">The object if conversion succeeded; otherwise <c>null</c>.</param>
/// <param name="error">The error if conversion failed; otherwise <c>null</c>.</param>
/// <returns>true if the conversion succeeded; otherwise, false.</returns>
public delegate bool TryGetFromKey(
   object? key,
   out object? obj,
   [MaybeNullWhen(true)] out object error);
