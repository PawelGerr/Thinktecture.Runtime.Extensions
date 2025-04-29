using System;
using System.Threading.Tasks;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public sealed partial class SmartEnum_NullableMembers
{
   public static readonly SmartEnum_NullableMembers Item1 = new(1, "Item1", TestFuncAsync);

   public string? Prop1 { get; }

   public Func<string?, Task<string?>?>? Prop2 { get; }

   private static Task<string?>? TestFuncAsync(string? arg)
   {
      return null;
   }
}
