namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TestClass>(IsValidatable = true)]
public partial struct TestSmartEnum_Struct_ClassBased
{
   public static readonly TestSmartEnum_Struct_ClassBased Value1 = new(new TestClass());
}
