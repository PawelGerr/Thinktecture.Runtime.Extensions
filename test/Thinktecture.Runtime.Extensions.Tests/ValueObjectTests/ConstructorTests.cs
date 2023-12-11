using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ConstructorTests
{
   [Fact]
   public void Keyed_value_object_with_public_ctor()
   {
      var obj = new KeyedValueObjectWithPublicCtor(42);
   }

   [Fact]
   public void Complex_value_object_with_public_ctor()
   {
      var obj = new ComplexValueObjectWithPublicCtor(1, 2);
   }
}
