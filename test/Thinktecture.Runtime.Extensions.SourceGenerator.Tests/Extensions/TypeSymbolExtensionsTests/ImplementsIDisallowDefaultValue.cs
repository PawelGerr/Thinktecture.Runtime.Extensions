namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class ImplementsIDisallowDefaultValue : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_type_implementing_interface_directly()
   {
      var src = @"
namespace Test;

public class MyType : Thinktecture.IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyType");

      var result = type.ImplementsIDisallowDefaultValue();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_type_implementing_interface_indirectly_via_base_class()
   {
      var src = @"
namespace Test;

public class BaseType : Thinktecture.IDisallowDefaultValue;

public class DerivedType : BaseType;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedType");

      var result = type.ImplementsIDisallowDefaultValue();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_type_implementing_interface_indirectly_via_another_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface : Thinktecture.IDisallowDefaultValue;

public class MyType : IMyInterface;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyType");

      var result = type.ImplementsIDisallowDefaultValue();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_type_not_implementing_interface()
   {
      var src = @"
namespace Test;

public class MyType;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyType");

      var result = type.ImplementsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_same_name_in_different_namespace()
   {
      var src = @"
namespace Other
{
   public interface IDisallowDefaultValue;
}

namespace Test
{
   public class MyType : Other.IDisallowDefaultValue;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyType");

      var result = type.ImplementsIDisallowDefaultValue();

      result.Should().BeFalse();
   }
}
