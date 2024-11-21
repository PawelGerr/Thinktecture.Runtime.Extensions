namespace Thinktecture.Runtime.Tests;

public ref struct TestRefStruct
{
   public object Value;

   public TestRefStruct(object value)
   {
      Value = value;
   }
}
