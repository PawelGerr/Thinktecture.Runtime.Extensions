namespace Thinktecture.Runtime.Tests.TestTypes;

[SmartEnum] // keyless: MUST be abstract with nested derived classes (mirrors SmartEnum_Keyless_DerivedTypes)
public abstract partial class Color
{
   public static readonly Color Red = new DerivedRed();
   public static readonly Color Green = new DerivedGreen();

   private sealed class DerivedRed : Color;
   private sealed class DerivedGreen : Color;
}
