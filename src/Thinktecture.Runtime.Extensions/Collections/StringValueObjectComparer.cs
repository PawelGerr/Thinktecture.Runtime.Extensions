namespace Thinktecture.Collections;

/// <summary>
/// Provides most common comparers for string-based value objects and smart enums.
/// </summary>
public sealed class StringValueObjectComparer<T> : IEqualityComparer<T>
   where T : IKeyedValueObject<string>
{
   /// <summary>
   /// Comparer using <see cref="StringComparer.Ordinal"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> Ordinal = new StringValueObjectComparer<T>(StringComparer.Ordinal);

   /// <summary>
   /// Comparer using <see cref="StringComparer.OrdinalIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> OrdinalIgnoreCase = new StringValueObjectComparer<T>(StringComparer.OrdinalIgnoreCase);

   /// <summary>
   /// Comparer using <see cref="StringComparer.CurrentCulture"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> CurrentCulture = new StringValueObjectComparer<T>(StringComparer.CurrentCulture);

   /// <summary>
   /// Comparer using <see cref="StringComparer.CurrentCultureIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> CurrentCultureIgnoreCase = new StringValueObjectComparer<T>(StringComparer.CurrentCultureIgnoreCase);

   /// <summary>
   /// Comparer using <see cref="StringComparer.InvariantCulture"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> InvariantCulture = new StringValueObjectComparer<T>(StringComparer.InvariantCulture);

   /// <summary>
   /// Comparer using <see cref="StringComparer.InvariantCultureIgnoreCase"/>.
   /// </summary>
   public static readonly IEqualityComparer<T> InvariantCultureIgnoreCase = new StringValueObjectComparer<T>(StringComparer.InvariantCultureIgnoreCase);

   private readonly IEqualityComparer<string> _comparer;

   private StringValueObjectComparer(IEqualityComparer<string> comparer)
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
