namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<string>(IsValidatable = true)]
public partial class EnumWithDuplicateKey
{
   public static readonly EnumWithDuplicateKey Item = new("Item");
   public static readonly EnumWithDuplicateKey Duplicate = new("item");
}
