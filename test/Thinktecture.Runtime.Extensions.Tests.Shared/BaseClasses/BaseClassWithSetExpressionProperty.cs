namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithSetExpressionProperty
{
   public static class Helper
   {
      public static int Property { get; set; }
   }

   public int Property
   {
      set => Helper.Property = value;
   }
}
