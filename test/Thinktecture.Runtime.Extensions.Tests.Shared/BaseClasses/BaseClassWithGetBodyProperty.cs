namespace Thinktecture.Runtime.Tests.BaseClasses;

public class BaseClassWithGetBodyProperty
{
   public int Property
   {
      get { return 42; }
      // ReSharper disable once ValueParameterNotUsed
      set { }
   }
}
