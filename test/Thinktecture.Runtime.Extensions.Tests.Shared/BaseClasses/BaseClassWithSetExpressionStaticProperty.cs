namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithSetExpressionStaticProperty
{
   public static class Helper
   {
      public static int Property { get; set; }
   }

   public static int Property
   {
      set => Helper.Property = value;
   }
}
