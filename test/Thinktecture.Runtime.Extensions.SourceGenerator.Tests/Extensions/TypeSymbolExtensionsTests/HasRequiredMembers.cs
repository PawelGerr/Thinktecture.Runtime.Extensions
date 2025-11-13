#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class HasRequiredMembers : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_type_with_required_property()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required string Name { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.HasRequiredMembers();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_type_with_required_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required string Name;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.HasRequiredMembers();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_type_without_required_members()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string Name { get; set; }
   public int Age;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.HasRequiredMembers();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_base_class_has_required_member()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public required string BaseName { get; set; }
}

public class DerivedClass : BaseClass
{
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = type.HasRequiredMembers();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_no_base_class_has_required_members()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public string BaseName { get; set; }
}

public class DerivedClass : BaseClass
{
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = type.HasRequiredMembers();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_with_multiple_levels_of_inheritance()
   {
      var src = @"
namespace Test;

public class GrandparentClass
{
   public required string GrandparentName { get; set; }
}

public class ParentClass : GrandparentClass
{
   public string ParentName { get; set; }
}

public class ChildClass : ParentClass
{
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.ChildClass");

      var result = type.HasRequiredMembers();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_type_with_only_optional_members()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public string? OptionalName { get; set; }
   public int Age { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.HasRequiredMembers();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_type_with_both_required_property_and_field()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public required string Name { get; set; }
   public required int Age;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.HasRequiredMembers();

      result.Should().BeTrue();
   }
}
