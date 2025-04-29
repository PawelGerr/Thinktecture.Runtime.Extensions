namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TestClass>]
public partial class SmartEnum_ClassBased
{
   public static readonly SmartEnum_ClassBased Value1 = new(new TestClass());
}
