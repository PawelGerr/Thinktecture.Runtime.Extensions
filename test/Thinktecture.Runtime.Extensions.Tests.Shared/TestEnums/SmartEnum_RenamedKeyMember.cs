namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(KeyMemberName = "_name")]
public partial class SmartEnum_RenamedKeyMember
{
   public static readonly SmartEnum_RenamedKeyMember Item1 = new("Item1");
}
