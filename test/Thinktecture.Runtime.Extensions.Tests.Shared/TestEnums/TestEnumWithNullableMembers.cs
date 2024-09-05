using System;
using System.Threading.Tasks;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public sealed partial class TestEnumWithNullableMembers
{
   public static readonly TestEnumWithNullableMembers Item1 = new(1, "Item1", TestFuncAsync);

   public string? Prop1 { get; }

   public Func<string?, Task<string?>?>? Prop2 { get; }

   // ReSharper disable once InconsistentNaming
   private static Task<string?>? TestFuncAsync(string? arg)
   {
      return null;
   }
}
