namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithGetBodyStaticProperty
{
   public static int Property
   {
      get { return 42; }
      // ReSharper disable once ValueParameterNotUsed
      set { }
   }
}
