using Thinktecture.Collections;

namespace Thinktecture;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
   /// <summary>
   /// Wraps the provided <paramref name="items"/> and exposes them as <see cref="IReadOnlyCollection{T}"/>.
   /// </summary>
   /// <param name="items">Items to expose as an <see cref="IReadOnlyCollection{T}"/>.</param>
   /// <param name="count">The number of <paramref name="items"/>.</param>
   /// <typeparam name="T">Item type.</typeparam>
   /// <returns>A wrapper around <paramref name="items"/> that implements <see cref="IReadOnlyCollection{T}"/>.</returns>
   /// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
   /// <exception cref="ArgumentException"><paramref name="count"/> is negative.</exception>
   public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items, int count)
   {
      return new EnumerableWrapperWithCount<T>(items, count);
   }
}
