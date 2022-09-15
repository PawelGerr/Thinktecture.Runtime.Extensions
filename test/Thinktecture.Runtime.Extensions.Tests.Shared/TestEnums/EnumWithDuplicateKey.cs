namespace Thinktecture.Runtime.Tests.TestEnums;

public sealed partial class EnumWithDuplicateKey : IValidatableEnum<string>
{
   public static readonly EnumWithDuplicateKey Item = new("Item");
   public static readonly EnumWithDuplicateKey Duplicate = new("item");
}
