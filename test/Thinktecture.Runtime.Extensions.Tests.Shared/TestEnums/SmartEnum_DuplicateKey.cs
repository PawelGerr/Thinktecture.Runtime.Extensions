namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
public partial class SmartEnum_DuplicateKey
{
   public static readonly SmartEnum_DuplicateKey Item = new("Item");
   public static readonly SmartEnum_DuplicateKey Duplicate = new("item");
}
