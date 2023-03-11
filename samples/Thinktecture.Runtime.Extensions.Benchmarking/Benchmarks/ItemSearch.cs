using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

[MemoryDiagnoser]
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
