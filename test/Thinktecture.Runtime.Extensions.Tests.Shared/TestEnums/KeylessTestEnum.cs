namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum]
public sealed partial class KeylessTestEnum
{
   public static readonly KeylessTestEnum Item1 = new(1);
   public static readonly KeylessTestEnum Item2 = new(2);

   public int IntProperty { get; }
}
