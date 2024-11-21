namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithGetExpressionStaticProperty
{
   public static int Property
   {
      get => 42;
      // ReSharper disable once ValueParameterNotUsed
      set { }
   }
}
