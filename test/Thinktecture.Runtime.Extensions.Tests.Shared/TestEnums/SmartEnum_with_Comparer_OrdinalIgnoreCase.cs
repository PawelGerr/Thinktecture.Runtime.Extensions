namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class SmartEnum_with_Comparer_OrdinalIgnoreCase
{
   public static readonly SmartEnum_with_Comparer_OrdinalIgnoreCase Item1 = new("Item1");
   public static readonly SmartEnum_with_Comparer_OrdinalIgnoreCase Item2 = new("item1");
}
