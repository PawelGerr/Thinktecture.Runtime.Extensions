namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(KeyMemberName = "_name",
                   IsValidatable = true)]
public partial class ValidatableTestEnumWithRenamedKeyMember
{
   public static readonly ValidatableTestEnumWithRenamedKeyMember Item1 = new("Item1");
}
