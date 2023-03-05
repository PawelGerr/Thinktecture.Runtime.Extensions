namespace Thinktecture;

/// <summary>
/// Contains some predefined comparer accessors.
/// </summary>
public static class ComparerAccessors
{
   /// <summary>
   /// Provides <see cref="StringComparer.Ordinal"/>.
   /// </summary>
   public class StringOrdinal : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.Ordinal;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.Ordinal;
   }

   /// <summary>
   /// Provides <see cref="StringComparer.OrdinalIgnoreCase"/>.
   /// </summary>
   public class StringOrdinalIgnoreCase : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.OrdinalIgnoreCase;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.OrdinalIgnoreCase;
   }

   /// <summary>
   /// Provides the default comparers.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class Default<T> : IEqualityComparerAccessor<T>, IComparerAccessor<T>
   {
      /// <inheritdoc />
      public static IEqualityComparer<T> EqualityComparer => EqualityComparer<T>.Default;

      /// <inheritdoc />
      public static IComparer<T> Comparer => Comparer<T>.Default;
   }
}
