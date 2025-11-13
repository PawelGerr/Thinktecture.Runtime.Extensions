namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(KeyMemberKind = MemberKind.Property)]
public partial class SmartEnum_with_KeyMemberKind_Property
{
   public static readonly SmartEnum_with_KeyMemberKind_Property Item1 = new("Item1");
   public static readonly SmartEnum_with_KeyMemberKind_Property Item2 = new("Item2");
}
