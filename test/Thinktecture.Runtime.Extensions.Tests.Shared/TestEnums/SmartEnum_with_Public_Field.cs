namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(KeyMemberKind = MemberKind.Field, KeyMemberAccessModifier = AccessModifier.Public)]
public partial class SmartEnum_with_Public_Field
{
   public static readonly SmartEnum_with_Public_Field Item1 = new(1);
   public static readonly SmartEnum_with_Public_Field Item2 = new(2);
}
