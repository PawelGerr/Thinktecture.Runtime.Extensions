using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Thinktecture.Runtime.Tests;

public class ReusableHashSetTests
{
   [Fact]
   public void Lease_returns_new_when_empty()
   {
      var pool = new IntSet(16);
      var cmp = new Eq1();

      var set = pool.Lease(cmp);

      Assert.NotNull(set);
      Assert.Same(cmp, set.Comparer);
      Assert.Empty(set);
   }

   [Fact]
   public void Return_then_Lease_returns_same_instance_with_matching_comparer()
   {
      var pool = new IntSet(16);
      var cmp = new Eq1();
      var set = new HashSet<int>(cmp) { 1, 2, 3 };

      pool.Return(set);

      var leased = pool.Lease(cmp);

      Assert.Same(set, leased);
      Assert.Empty(leased); // cleared by Return
   }

   [Fact]
   public void Return_drops_when_too_large()
   {
      var pool = new IntSet(2);
      var cmp = new Eq1();
      var set = new HashSet<int>(cmp) { 1, 2, 3 };

      pool.Return(set);

      var leased = pool.Lease(cmp);

      Assert.NotSame(set, leased);
   }

   [Fact]
   public void Lease_with_different_comparer_does_not_consume_cache()
   {
      var pool = new IntSet(16);
      var cmp1 = new Eq1();
      var cmp2 = new Eq2();
      var set = new HashSet<int>(cmp1);

      pool.Return(set);

      var leasedWithOther = pool.Lease(cmp2);
      Assert.NotSame(set, leasedWithOther);

      // Cache should still contain set;
      var leasedWithSame = pool.Lease(cmp1);
      Assert.Same(set, leasedWithSame);
   }

   [Fact]
   public async Task Concurrent_Lease_consumes_cache_at_most_once()
   {
      var pool = new IntSet(16);
      var cmp = new Eq1();
      var cached = new HashSet<int>(cmp);

      pool.Return(cached);

      var winners = 0;
      var tasks = Enumerable.Range(0, Environment.ProcessorCount * 2).Select(async _ =>
      {
         await Task.Yield();
         var s = pool.Lease(cmp);
         if (ReferenceEquals(s, cached))
            Interlocked.Increment(ref winners);
      });

      await Task.WhenAll(tasks);

      Assert.Equal(1, winners);
   }

   [Fact]
   public void Return_does_not_overwrite_existing_cache()
   {
      var pool = new IntSet(16);
      var cmp = new Eq1();
      var a = new HashSet<int>(cmp) { 1 };
      var b = new HashSet<int>(cmp) { 2 };

      pool.Return(a);
      pool.Return(b); // should be dropped because slot is not empty

      var leased = pool.Lease(cmp);
      Assert.Same(a, leased);
   }

   private sealed class IntSet(int max) : ReusableHashSet<int>(max);

   private sealed class Eq1 : IEqualityComparer<int>
   {
      public bool Equals(int x, int y) => x == y;
      public int GetHashCode(int obj) => obj;
   }

   private sealed class Eq2 : IEqualityComparer<int>
   {
      public bool Equals(int x, int y) => x == y;
      public int GetHashCode(int obj) => obj;
   }
}
