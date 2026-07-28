namespace Thinktecture;

/// <summary>
/// Contains some predefined comparer accessors.
/// </summary>
/// <remarks>
/// An accessor declared here that can be used for a <see cref="string"/> key member must return a comparer
/// that implements <c>IAlternateEqualityComparer&lt;ReadOnlySpan&lt;char&gt;, string&gt;</c> on .NET 9 and
/// higher. The reason is that the source generator recognizes accessors of this class by their namespace and
/// then generates the allocation-free span-based lookup for string-keyed Smart Enums without checking the
/// capability at runtime. An accessor that breaks this rule would make the generated lookup throw. Key types
/// other than <see cref="string"/> are not affected, because no span-based lookup is generated for them.
/// </remarks>
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
