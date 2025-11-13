using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class GetGenericTypeDefinition : CompilationTestBase
{
   [Fact]
   public void Returns_unbound_generic_for_bound_generic_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class MyClass
{
   public List<int> MyList { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("MyList").First() as IPropertySymbol;
      var listOfInt = property!.Type as INamedTypeSymbol;

      var result = listOfInt!.GetGenericTypeDefinition();

      result.IsUnboundGenericType.Should().BeTrue();
      result.Name.Should().Be("List");
      result.Arity.Should().Be(1);
   }

   [Fact]
   public void Returns_self_for_non_generic_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetGenericTypeDefinition();

      result.Should().BeSameAs(type);
   }

   [Fact]
   public void Returns_unbound_for_already_unbound_generic_type()
   {
      var src = @"
namespace Test;

public class MyGenericClass<T>;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyGenericClass`1");

      var result = type.GetGenericTypeDefinition();

      result.Should().NotBeSameAs(type);
      result.IsUnboundGenericType.Should().BeTrue();
      result.Name.Should().Be("MyGenericClass");
      result.Arity.Should().Be(1);
   }

   [Fact]
   public void Returns_unbound_for_constructed_generic_with_multiple_type_parameters()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class MyClass
{
   public Dictionary<string, int> MyDict { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("MyDict").First() as IPropertySymbol;
      var dictOfStringInt = property!.Type as INamedTypeSymbol;

      var result = dictOfStringInt!.GetGenericTypeDefinition();

      result.IsUnboundGenericType.Should().BeTrue();
      result.Name.Should().Be("Dictionary");
      result.Arity.Should().Be(2);
   }

   [Fact]
   public void Returns_unbound_for_nested_generic_type()
   {
      var src = @"
namespace Test;

public class OuterClass<T>
{
   public class InnerClass<U>
   {
      public T TValue { get; set; }
      public U UValue { get; set; }
   }
}

public class Usage
{
   public OuterClass<int>.InnerClass<string> MyField;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers("MyField").First() as IFieldSymbol;
      var nestedGeneric = field!.Type as INamedTypeSymbol;

      var result = nestedGeneric!.GetGenericTypeDefinition();

      result.IsUnboundGenericType.Should().BeTrue();
      result.Arity.Should().Be(1); // InnerClass has 1 type parameter
   }

   [Fact]
   public void Handles_special_types_correctly()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.GetGenericTypeDefinition();

      result.Should().BeSameAs(intType);
   }
}
