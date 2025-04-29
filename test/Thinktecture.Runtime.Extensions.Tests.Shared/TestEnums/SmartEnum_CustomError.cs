namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ValidationError<CustomValidationError>]
public partial class TestSmartEnum_CustomError
{
   public static readonly TestSmartEnum_CustomError Item1 = new("item1");
   public static readonly TestSmartEnum_CustomError Item2 = new("item2");
}
