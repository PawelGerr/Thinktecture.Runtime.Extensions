namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithGetExpressionProperty
{
   public int Property
   {
      get => 42;
      // ReSharper disable once ValueParameterNotUsed
      set { }
   }
}
