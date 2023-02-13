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
}
