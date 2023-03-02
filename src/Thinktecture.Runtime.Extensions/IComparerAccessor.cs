namespace Thinktecture;

/// <summary>
/// Provides an <see cref="IComparer{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the object to compare.</typeparam>
public interface IComparerAccessor<in T>
{
   /// <summary>
   /// A comparer.
   /// </summary>
   static abstract IComparer<T> Comparer { get; }
}
