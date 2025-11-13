using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class FindDerivedInnerTypes : CompilationTestBase
{
   [Fact]
   public void Returns_empty_for_type_with_no_inner_types()
   {
      var src = @"
namespace Test;

public class BaseClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_single_derived_inner_class()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class Derived : BaseClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("Derived");
      result[0].Level.Should().Be(1);
   }

   [Fact]
   public void Returns_multiple_derived_inner_classes_at_same_level()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class Derived1 : BaseClass;
   public class Derived2 : BaseClass;
   public class Derived3 : BaseClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(3);
      result.Select(d => d.Type.Name).Should().BeEquivalentTo("Derived1", "Derived2", "Derived3");
      result.Should().OnlyContain(d => d.Level == 1);
   }

   [Fact]
   public void Excludes_non_class_types()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public struct DerivedStruct;
   public interface IDerivedInterface;
   public enum DerivedEnum { Value }
   public class DerivedClass : BaseClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("DerivedClass");
   }

   [Fact]
   public void Finds_derived_types_at_multiple_nesting_levels()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class Level1Derived : BaseClass
   {
      public class Level2Derived : BaseClass;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(2);
      var level1 = result.Single(d => d.Type.Name == "Level1Derived");
      var level2 = result.Single(d => d.Type.Name == "Level2Derived");

      level1.Level.Should().Be(1);
      level2.Level.Should().Be(2);
   }

   [Fact]
   public void Handles_deep_nesting_correctly()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class Level1 : BaseClass
   {
      public class Level2 : BaseClass
      {
         public class Level3 : BaseClass;
      }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(3);
      result.Single(d => d.Type.Name == "Level1").Level.Should().Be(1);
      result.Single(d => d.Type.Name == "Level2").Level.Should().Be(2);
      result.Single(d => d.Type.Name == "Level3").Level.Should().Be(3);
   }

   [Fact]
   public void Finds_derived_types_implementing_interface()
   {
      var src = @"
namespace Test;

public interface IBaseInterface
{
   public class Derived : IBaseInterface;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.IBaseInterface");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("Derived");
   }

   [Fact]
   public void Handles_generic_base_types()
   {
      var src = @"
namespace Test;

public class BaseClass<T>
{
   public class Derived : BaseClass<int>;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass`1");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("Derived");
   }

   [Fact]
   public void Excludes_types_not_deriving_from_base()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class Derived : BaseClass;
   public class NotDerived;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("Derived");
   }

   [Fact]
   public void Finds_types_in_nested_inner_types_not_directly_derived()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public class NotDerived
   {
      public class DerivedNested : BaseClass;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.BaseClass");

      var result = type.FindDerivedInnerTypes();

      result.Should().HaveCount(1);
      result[0].Type.Name.Should().Be("DerivedNested");
      result[0].Level.Should().Be(2); // Nested 2 levels deep
   }
}
