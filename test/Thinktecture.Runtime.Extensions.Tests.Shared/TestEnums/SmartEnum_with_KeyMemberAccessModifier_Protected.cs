namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(KeyMemberAccessModifier = AccessModifier.Internal)]
public partial class SmartEnum_with_KeyMemberAccessModifier_Internal
{
   public static readonly SmartEnum_with_KeyMemberAccessModifier_Internal Item1 = new("Item1");
   public static readonly SmartEnum_with_KeyMemberAccessModifier_Internal Item2 = new("Item2");
}
