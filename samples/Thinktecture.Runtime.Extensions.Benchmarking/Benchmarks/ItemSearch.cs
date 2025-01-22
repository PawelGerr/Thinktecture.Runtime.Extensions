using System;
using System.Collections.Frozen;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

22.01.2025

.NET 9.0.1

| Method                    | SearchTerm | Mean      | Error     | StdDev    | Median    | Allocated |
|-------------------------- |----------- |----------:|----------:|----------:|----------:|----------:|
| Dictionary                | aaaaAaaaaa | 16.693 ns | 1.1213 ns | 3.3061 ns | 17.322 ns |         - |
| FrozenDictionary          | aaaaAaaaaa |  6.703 ns | 0.1758 ns | 0.1644 ns |  6.683 ns |         - |
| ReadOnlyDictionary        | aaaaAaaaaa | 12.893 ns | 0.3104 ns | 0.6341 ns | 12.690 ns |         - |
| ImmutableDictionary       | aaaaAaaaaa | 17.369 ns | 0.2967 ns | 0.2478 ns | 17.365 ns |         - |
| ImmutableSortedDictionary | aaaaAaaaaa | 17.929 ns | 0.4007 ns | 0.9601 ns | 17.715 ns |         - |
| ArrayIteration            | aaaaAaaaaa |  2.973 ns | 0.1116 ns | 0.1044 ns |  2.937 ns |         - |

| Dictionary                | iiiiIiiiii |  9.904 ns | 0.2482 ns | 0.2438 ns |  9.821 ns |         - |
| FrozenDictionary          | iiiiIiiiii |  7.024 ns | 0.1959 ns | 0.5363 ns |  6.867 ns |         - |
| ReadOnlyDictionary        | iiiiIiiiii | 12.080 ns | 0.2953 ns | 0.6544 ns | 11.967 ns |         - |
| ImmutableDictionary       | iiiiIiiiii | 17.562 ns | 0.3981 ns | 0.7280 ns | 17.336 ns |         - |
| ImmutableSortedDictionary | iiiiIiiiii | 22.474 ns | 0.4374 ns | 0.7064 ns | 22.262 ns |         - |
| ArrayIteration            | iiiiIiiiii | 21.703 ns | 0.4796 ns | 0.6402 ns | 21.619 ns |         - |

| Dictionary                | tttttTtttt |  9.533 ns | 0.2432 ns | 0.3928 ns |  9.524 ns |         - |
| FrozenDictionary          | tttttTtttt |  6.943 ns | 0.1874 ns | 0.2370 ns |  6.945 ns |         - |
| ReadOnlyDictionary        | tttttTtttt | 11.460 ns | 0.2830 ns | 0.4570 ns | 11.473 ns |         - |
| ImmutableDictionary       | tttttTtttt | 17.708 ns | 0.3967 ns | 0.4245 ns | 17.626 ns |         - |
| ImmutableSortedDictionary | tttttTtttt | 23.327 ns | 0.5152 ns | 1.5030 ns | 22.802 ns |         - |
| ArrayIteration            | tttttTtttt | 47.615 ns | 0.9459 ns | 0.9714 ns | 47.931 ns |         - |


 */

public class ItemSearch
{
   public class SmartEnum
   {
      public string Key { get; }

      public SmartEnum(string key)
      {
         Key = key;
      }
   }

   private readonly Dictionary<string, SmartEnum> _dictionary;
   private readonly FrozenDictionary<string, SmartEnum> _frozenDictionary;
   private readonly ReadOnlyDictionary<string, SmartEnum> _readOnlyDictionary;
   private readonly ImmutableDictionary<string, SmartEnum> _immutableDictionary;
   private readonly ImmutableSortedDictionary<string, SmartEnum> _immutableSortedDictionary;
   private readonly SmartEnum[] _array;

   // ReSharper disable NotAccessedField.Local
   private readonly List<SmartEnum> _list;
   private readonly ReadOnlyCollection<SmartEnum> _readOnlyCollection;
   private readonly string[] _keysArray;
   // ReSharper restore NotAccessedField.Local

   [Params("aaaaAaaaaa", "iiiiIiiiii", "tttttTtttt")]
   public string SearchTerm { get; set; } = String.Empty;

   public ItemSearch()
   {
      var dictionary = new Dictionary<string, SmartEnum>(10, StringComparer.OrdinalIgnoreCase);
      _immutableDictionary = ImmutableDictionary<string, SmartEnum>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
      _immutableSortedDictionary = ImmutableSortedDictionary<string, SmartEnum>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
      var list = new List<SmartEnum>(10);

      for (var i = 0; i < 20; i++)
      {
         var key = new string((char)('a' + i), 10);
         var item = new SmartEnum(key);
         dictionary.Add(item.Key, item);
         _immutableDictionary = _immutableDictionary.Add(item.Key, item);
         _immutableSortedDictionary = _immutableSortedDictionary.Add(item.Key, item);
         list.Add(item);
      }

      _dictionary = dictionary;
      _frozenDictionary = _dictionary.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
      _readOnlyDictionary = new ReadOnlyDictionary<string, SmartEnum>(dictionary);
      _list = list;
      _readOnlyCollection = list.AsReadOnly();
      _array = list.OrderBy(i => i.Key).ToArray();
      _keysArray = _array.Select(i => i.Key).ToArray();
   }

   [Benchmark]
   public SmartEnum? Dictionary()
   {
      _dictionary.TryGetValue(SearchTerm, out var item);

      return item;
   }

   [Benchmark]
   public SmartEnum? FrozenDictionary()
   {
      _frozenDictionary.TryGetValue(SearchTerm, out var item);

      return item;
   }

   [Benchmark]
   public SmartEnum? ReadOnlyDictionary()
   {
      _readOnlyDictionary.TryGetValue(SearchTerm, out var item);

      return item;
   }

   [Benchmark]
   public SmartEnum? ImmutableDictionary()
   {
      _immutableDictionary.TryGetValue(SearchTerm, out var item);

      return item;
   }

   [Benchmark]
   public SmartEnum? ImmutableSortedDictionary()
   {
      _immutableSortedDictionary.TryGetValue(SearchTerm, out var item);

      return item;
   }

   // [Benchmark]
   // public SmartEnum? List()
   // {
   //    for (var i = 0; i < _list.Count; i++)
   //    {
   //       var item = _list[i];
   //
   //       if (SearchTerm.Equals(item.Key, StringComparison.OrdinalIgnoreCase))
   //          return item;
   //    }
   //
   //    return null;
   // }
   //
   // [Benchmark]
   // public SmartEnum? ListAsSpan()
   // {
   //    var span = CollectionsMarshal.AsSpan(_list);
   //
   //    for (var i = 0; i < span.Length; i++)
   //    {
   //       var item = span[i];
   //
   //       if (SearchTerm.Equals(item.Key, StringComparison.OrdinalIgnoreCase))
   //          return item;
   //    }
   //
   //    return null;
   // }

   // [Benchmark]
   // public SmartEnum? ReadOnlyList()
   // {
   //    for (var i = 0; i < _readOnlyCollection.Count; i++)
   //    {
   //       var item = _readOnlyCollection[i];
   //
   //       if (SearchTerm.Equals(item.Key, StringComparison.OrdinalIgnoreCase))
   //          return item;
   //    }
   //
   //    return null;
   // }

   [Benchmark]
   public SmartEnum? ArrayIteration()
   {
      for (var i = 0; i < _array.Length; i++)
      {
         var item = _array[i];

         if (SearchTerm.Equals(item.Key, StringComparison.OrdinalIgnoreCase))
            return item;
      }

      return null;
   }

   // [Benchmark]
   // public SmartEnum? ArrayBinary()
   // {
   //    var index = Array.BinarySearch(_keysArray, SearchTerm, StringComparer.OrdinalIgnoreCase);
   //
   //    if (index >= 0)
   //       return _array[index];
   //
   //    return null;
   // }
}
