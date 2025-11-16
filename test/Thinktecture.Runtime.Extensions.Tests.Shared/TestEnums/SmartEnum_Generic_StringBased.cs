using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_Generic_StringBased<T>
   where T : IComparable<T>
{
   public static readonly SmartEnum_Generic_StringBased<T> Item1 = new("item1", default);
   public static readonly SmartEnum_Generic_StringBased<T> Item2 = new("item2", default);

   public T? Value { get; }
}
