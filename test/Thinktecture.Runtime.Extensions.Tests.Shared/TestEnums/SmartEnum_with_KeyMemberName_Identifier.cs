namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(KeyMemberName = "Identifier")]
public partial class SmartEnum_with_KeyMemberName_Identifier
{
   public static readonly SmartEnum_with_KeyMemberName_Identifier Item1 = new(1);
   public static readonly SmartEnum_with_KeyMemberName_Identifier Item2 = new(2);
}
