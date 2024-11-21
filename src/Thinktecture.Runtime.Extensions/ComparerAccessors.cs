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
   /// Provides <see cref="StringComparer.CurrentCulture"/>.
   /// </summary>
   public class CurrentCulture : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.CurrentCulture;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.CurrentCulture;
   }

   /// <summary>
   /// Provides <see cref="StringComparer.CurrentCultureIgnoreCase"/>.
   /// </summary>
   public class CurrentCultureIgnoreCase : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.CurrentCultureIgnoreCase;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.CurrentCultureIgnoreCase;
   }

   /// <summary>
   /// Provides <see cref="StringComparer.InvariantCulture"/>.
   /// </summary>
   public class InvariantCulture : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.InvariantCulture;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.InvariantCulture;
   }

   /// <summary>
   /// Provides <see cref="StringComparer.InvariantCultureIgnoreCase"/>.
   /// </summary>
   public class InvariantCultureIgnoreCase : IEqualityComparerAccessor<string>, IComparerAccessor<string>
   {
      /// <inheritdoc />
      public static IEqualityComparer<string> EqualityComparer => StringComparer.InvariantCultureIgnoreCase;

      /// <inheritdoc />
      public static IComparer<string> Comparer => StringComparer.InvariantCultureIgnoreCase;
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
