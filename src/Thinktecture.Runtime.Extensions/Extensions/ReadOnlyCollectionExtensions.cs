namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="IReadOnlyCollection{T}"/>.
/// </summary>
public static class ReadOnlyCollectionExtensions
{
   /// <summary>
   /// Projects and wraps the provided <paramref name="items"/> and exposes the projected values as <see cref="IReadOnlyCollection{TResult}"/>.
   /// </summary>
   /// <param name="items">Items to project to <see cref="IReadOnlyCollection{TResult}"/>.</param>
   /// <param name="selector">Returns a item of type <typeparamref name="TResult"/> for each item of type <typeparamref name="T"/>.</param>
   /// <typeparam name="T">Item type.</typeparam>
   /// <typeparam name="TResult">Result type.</typeparam>
   /// <returns>A collection of type <see cref="IReadOnlyCollection{TResult}"/>.</returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="items"/> or <paramref name="selector"/> is <c>null</c>.
   /// </exception>
   public static IReadOnlyCollection<TResult> ToReadOnlyCollection<T, TResult>(this IReadOnlyCollection<T> items, Func<T, TResult> selector)
   {
      return items.Select(selector).ToReadOnlyCollection(items.Count);
   }
}
