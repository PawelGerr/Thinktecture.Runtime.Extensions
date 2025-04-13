namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TestClass>]
public partial class TestSmartEnum_Class_ClassBased
{
   public static readonly TestSmartEnum_Class_ClassBased Value1 = new(new TestClass());
}
