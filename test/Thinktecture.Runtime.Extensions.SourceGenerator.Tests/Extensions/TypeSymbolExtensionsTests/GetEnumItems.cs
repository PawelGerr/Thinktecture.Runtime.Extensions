using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class GetEnumItems : CompilationTestBase
{
   [Fact]
   public void Returns_all_static_fields_of_enum_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;
   public static readonly MyEnum Item2 = default!;
   public static readonly MyEnum Item3 = default!;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      result.Should().HaveCount(3);
      result.Select(f => f.Name).Should().BeEquivalentTo("Item1", "Item2", "Item3");
   }

   [Fact]
   public void Returns_empty_for_enum_with_no_items()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Excludes_instance_fields()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;
   public readonly int InstanceField = 42;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Item1");
   }

   [Fact]
   public void Excludes_static_fields_of_different_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;
   public static readonly int NotAnEnumItem = 42;
   public static readonly string AlsoNotAnEnumItem = ""test"";
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Item1");
   }

   [Fact]
   public void Excludes_ignored_fields()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;

   [IgnoreMember]
   public static readonly MyEnum IgnoredItem = default!;
}
";
      var compilation = CreateCompilation(src, additionalReferences:
      [
         typeof(SmartEnumAttribute<>).Assembly.Location,
         typeof(IgnoreMemberAttribute).Assembly.Location
      ]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Item1");
   }

   [Fact]
   public void Excludes_implicitly_declared_fields()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;

   // Auto-property backing field will be implicitly declared
   public int SomeProperty { get; set; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      // Should only get Item1, not the backing field for SomeProperty
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Item1");
   }

   [Fact]
   public void Handles_const_fields_correctly()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum
{
   public static readonly MyEnum Item1 = default!;
   public const int ConstValue = 42;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.GetEnumItems();

      // Const fields are static but should be excluded (different type)
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Item1");
   }
}
