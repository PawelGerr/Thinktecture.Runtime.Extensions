namespace Thinktecture.Collections;

/// <summary>
/// Provides the most common comparers for string-based value objects and smart enums.
/// </summary>
public sealed class StringKeyedObjectComparer<T> : IEqualityComparer<T>
   where T : IConvertible<string>
{
   /// <summary>
   /// Comparer using <see cref="StringComparer.Ordinal"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> Ordinal = new StringKeyedObjectComparer<T>(StringComparer.Ordinal);

   /// <summary>
   /// Comparer using <see cref="StringComparer.OrdinalIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> OrdinalIgnoreCase = new StringKeyedObjectComparer<T>(StringComparer.OrdinalIgnoreCase);

   /// <summary>
   /// Comparer using <see cref="StringComparer.CurrentCulture"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> CurrentCulture = new StringKeyedObjectComparer<T>(StringComparer.CurrentCulture);

   /// <summary>
   /// Comparer using <see cref="StringComparer.CurrentCultureIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> CurrentCultureIgnoreCase = new StringKeyedObjectComparer<T>(StringComparer.CurrentCultureIgnoreCase);

   /// <summary>
   /// Comparer using <see cref="StringComparer.InvariantCulture"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> InvariantCulture = new StringKeyedObjectComparer<T>(StringComparer.InvariantCulture);

   /// <summary>
   /// Comparer using <see cref="StringComparer.InvariantCultureIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> InvariantCultureIgnoreCase = new StringKeyedObjectComparer<T>(StringComparer.InvariantCultureIgnoreCase);

   private readonly IEqualityComparer<string> _comparer;

   private StringKeyedObjectComparer(IEqualityComparer<string> comparer)
   {
      _comparer = comparer;
   }

   /// <inheritdoc />
   public bool Equals(T? x, T? y)
   {
      return _comparer.Equals(x?.ToValue(), y?.ToValue());
   }

   /// <inheritdoc />
   public int GetHashCode(T obj)
   {
      return _comparer.GetHashCode(obj.ToValue());
   }
}
