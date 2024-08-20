namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(KeyMemberName = "_name")]
public partial class TestEnumWithRenamedKeyMember
{
   public static readonly TestEnumWithRenamedKeyMember Item1 = new("Item1");
}
