using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_Generic_IntBased<T>
   where T : IEquatable<T>
{
   public static readonly SmartEnum_Generic_IntBased<T> Item1 = new(1, default);
   public static readonly SmartEnum_Generic_IntBased<T> Item2 = new(2, default);
   public static readonly SmartEnum_Generic_IntBased<T> Item3 = new(3, default);

   public T? Value { get; }
}
