namespace Thinktecture;

/// <summary>
/// Provides an <see cref="IEqualityComparer{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the object to compare.</typeparam>
public interface IEqualityComparerAccessor<in T>
{
   /// <summary>
   /// An equality comparer.
   /// </summary>
   static abstract IEqualityComparer<T> EqualityComparer { get; }
}
