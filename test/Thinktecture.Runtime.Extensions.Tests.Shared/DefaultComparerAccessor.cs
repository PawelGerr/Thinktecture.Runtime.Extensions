using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests;

public class DefaultComparerAccessor<T> : IEqualityComparerAccessor<T>, IComparerAccessor<T>
{
   public static IEqualityComparer<T> EqualityComparer => EqualityComparer<T>.Default;

   public static IComparer<T> Comparer => Comparer<T>.Default;
}
