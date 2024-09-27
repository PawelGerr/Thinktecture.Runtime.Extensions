namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>]
public partial class TestEnumWithArrayType
{
   private static readonly TestEnumWithArrayType[] InternalMaritalStatusWithPartner = [];

   public static readonly TestEnumWithArrayType Item = new(1);
}
