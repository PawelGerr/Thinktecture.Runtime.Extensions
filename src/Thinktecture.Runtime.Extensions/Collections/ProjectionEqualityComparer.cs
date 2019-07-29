using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Thinktecture.Collections
{
   /// <summary>
   /// Generic comparer for instance of type <typeparamref name="T"/>.
   /// </summary>
   /// <typeparam name="T">Type of the items to compare.</typeparam>
   /// <typeparam name="TItem">Type being compared.</typeparam>
   public class ProjectionEqualityComparer<T, TItem> : IEqualityComparer<T>
   {
      private readonly IEqualityComparer<TItem> _comparer;
      private readonly Func<T, TItem> _selector;

      /// <summary>
      /// Initializes new instance of <see cref="ProjectionEqualityComparer{T,TItem}"/>.
      /// <remarks>The comparer <see cref="EqualityComparer{TItem}.Default"/> is being used for comparison.</remarks>
      /// </summary>
      /// <param name="selector">Selector returning an instance of type <typeparamref name="TItem"/> to use for comparison.</param>
      public ProjectionEqualityComparer([NotNull] Func<T, TItem> selector)
         : this(selector, EqualityComparer<TItem>.Default)
      {
      }

      /// <summary>
      /// Initializes new instance of <see cref="ProjectionEqualityComparer{T,TItem}"/>.
      /// </summary>
      /// <param name="selector">Selector.</param>
      /// <param name="comparer">Comparer to use.</param>
      public ProjectionEqualityComparer([NotNull] Func<T, TItem> selector, [NotNull] IEqualityComparer<TItem> comparer)
      {
         _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
         _selector = selector ?? throw new ArgumentNullException(nameof(selector));
      }

      /// <inheritdoc />
      public bool Equals(T x, T y)
      {
         return _comparer.Equals(_selector(x), _selector(y));
      }

      /// <inheritdoc />
      public int GetHashCode(T obj)
      {
         return _comparer.GetHashCode(_selector(obj));
      }
   }
}
