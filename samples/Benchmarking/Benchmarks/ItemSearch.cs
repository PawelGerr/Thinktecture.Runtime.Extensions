using System;
using System.Collections.Frozen;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

29.04.2025

.NET 9.0.4

| Method                    | SearchTerm | Mean      | Error     | StdDev    | Allocated |
|-------------------------- |----------- |----------:|----------:|----------:|----------:|
| Dictionary                | aaaaAaaaaa |  9.662 ns | 0.2411 ns | 0.3380 ns |         - |
| FrozenDictionary          | aaaaAaaaaa |  6.492 ns | 0.1784 ns | 0.2615 ns |         - |
| ReadOnlyDictionary        | aaaaAaaaaa | 11.174 ns | 0.0698 ns | 0.0583 ns |         - |
| ImmutableDictionary       | aaaaAaaaaa | 15.708 ns | 0.1943 ns | 0.1817 ns |         - |
| ImmutableSortedDictionary | aaaaAaaaaa | 17.891 ns | 0.4092 ns | 0.8078 ns |         - |
| ArrayIteration            | aaaaAaaaaa |  2.658 ns | 0.1031 ns | 0.1666 ns |         - |

| Dictionary                | iiiiIiiiii |  9.338 ns | 0.2343 ns | 0.2406 ns |         - |
| FrozenDictionary          | iiiiIiiiii |  6.558 ns | 0.1823 ns | 0.2238 ns |         - |
| ReadOnlyDictionary        | iiiiIiiiii | 10.186 ns | 0.2542 ns | 0.2927 ns |         - |
| ImmutableDictionary       | iiiiIiiiii | 15.271 ns | 0.3567 ns | 0.3663 ns |         - |
| ImmutableSortedDictionary | iiiiIiiiii | 20.448 ns | 0.4550 ns | 0.7217 ns |         - |
| ArrayIteration            | iiiiIiiiii | 19.685 ns | 0.4388 ns | 0.5706 ns |         - |

| Dictionary                | tttttTtttt |  9.221 ns | 0.2357 ns | 0.3670 ns |         - |
| FrozenDictionary          | tttttTtttt |  6.200 ns | 0.1754 ns | 0.4464 ns |         - |
| ReadOnlyDictionary        | tttttTtttt | 10.563 ns | 0.1320 ns | 0.1235 ns |         - |
| ImmutableDictionary       | tttttTtttt | 17.622 ns | 0.4002 ns | 0.7899 ns |         - |
| ImmutableSortedDictionary | tttttTtttt | 21.024 ns | 0.2958 ns | 0.2470 ns |         - |
| ArrayIteration            | tttttTtttt | 44.569 ns | 0.9428 ns | 1.7708 ns |         - |

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
