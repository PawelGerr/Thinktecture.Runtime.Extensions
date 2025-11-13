namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(KeyMemberAccessModifier = AccessModifier.Public)]
public partial class SmartEnum_with_KeyMemberAccessModifier_Public
{
   public static readonly SmartEnum_with_KeyMemberAccessModifier_Public Item1 = new(1);
   public static readonly SmartEnum_with_KeyMemberAccessModifier_Public Item2 = new(2);
}
