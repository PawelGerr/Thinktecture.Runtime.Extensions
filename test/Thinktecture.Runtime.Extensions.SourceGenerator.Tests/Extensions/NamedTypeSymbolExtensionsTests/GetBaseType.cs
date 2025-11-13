using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetBaseType : CompilationTestBase
{
   [Fact]
   public void Returns_base_type_state_for_class_with_base_in_same_assembly()
   {
      var src = @"
namespace Test;

public class BaseClass;

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_class_with_base_in_different_assembly()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase
{
   public ExternalBase() { }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, "ExternalLib");
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, "TestLib", additionalReferences: baseReference);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().NotBeNull();
   }

   [Fact]
   public void Returns_null_for_struct_with_valuetype_base()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public int Value { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");

      var result = structType.GetBaseType(factory);

      // Structs have System.ValueType as their base type
      result.Should().BeNull();
   }

   [Fact]
   public void Returns_null_for_interface_without_base()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
   void DoSomething();
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyInterface");

      var result = interfaceType.GetBaseType(factory);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_record_with_base()
   {
      var src = @"
namespace Test;

public record BaseRecord(string Name);

public record DerivedRecord(string Name, int Age) : BaseRecord(Name);
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedRecord");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_generic_class_with_base()
   {
      var src = @"
namespace Test;

public class BaseClass<T>;

public class DerivedClass : BaseClass<int>;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_nested_class_with_base()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class NestedBase
   {
   }

   public class NestedDerived : NestedBase
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.OuterClass+NestedDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_multi_level_inheritance_chain()
   {
      var src = @"
namespace Test;

public class GrandParent;

public class Parent : GrandParent;

public class Child : Parent;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var childType = GetTypeSymbol(compilation, "Test.Child");

      var result = childType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   // Edge cases
   [Fact]
   public void Returns_null_for_class_with_object_base()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = classType.GetBaseType(factory);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_null_for_class_explicitly_inheriting_from_object()
   {
      var src = @"
namespace Test;

public class MyClass : object;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = classType.GetBaseType(factory);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_null_when_base_type_is_error_type()
   {
      var src = @"
namespace Test;

public class DerivedClass : NonExistentBaseClass;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentBaseClass' could not be found (are you missing a using directive or an assembly reference?)"]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().BeNull();
   }

   [Fact]
   public void Returns_null_for_enum_type_with_enum_base()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   Value1,
   Value2
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var enumType = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = enumType.GetBaseType(factory);

      // Enums have System.Enum as their base type
      result.Should().BeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_nested_class_with_base_in_different_assembly()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase;
";
      var derivedAssemblySrc = @"
namespace Test;

public class OuterClass
{
   public class NestedDerived : ExternalLib.ExternalBase
   {
   }
}
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, "ExternalLib");
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, "TestLib", additionalReferences: baseReference);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.OuterClass+NestedDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_null_for_delegate_type_with_multicastdelegate_base()
   {
      var src = @"
namespace Test;

public delegate void MyDelegate(int value);
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var delegateType = GetTypeSymbol(compilation, "Test.MyDelegate");

      var result = delegateType.GetBaseType(factory);

      // Delegates have System.MulticastDelegate as their base type
      result.Should().BeNull();
   }

   // Complex scenarios
   [Fact]
   public void Returns_base_type_state_with_internals_visible_to_attribute()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestLib"")]

namespace ExternalLib;

internal class InternalBase;

internal class PublicDerived : InternalBase;
";
      var derivedAssemblySrc = @"
namespace Test;

internal class DerivedClass : ExternalLib.PublicDerived;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, "ExternalLib");
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, "TestLib", additionalReferences: baseReference);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_generic_base_with_constraints()
   {
      var src = @"
namespace Test;

public class BaseClass<T> where T : class;

public class DerivedClass : BaseClass<string>;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_null_for_record_struct_with_valuetype_base()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct(int Value);
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var recordStructType = GetTypeSymbol(compilation, "Test.MyRecordStruct");

      var result = recordStructType.GetBaseType(factory);

      // Record structs also have System.ValueType
      result.Should().BeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_abstract_class_with_base()
   {
      var src = @"
namespace Test;

public abstract class AbstractBase
{
   public abstract void DoSomething();
}

public class ConcreteClass : AbstractBase
{
   public override void DoSomething() { }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var concreteType = GetTypeSymbol(compilation, "Test.ConcreteClass");

      var result = concreteType.GetBaseType(factory);

      result.Should().NotBeNull();
   }

   [Fact]
   public void Returns_base_type_state_for_nested_in_generic_containing_with_generic_base()
   {
      var src = @"
namespace Test;

public class Base<T>;

public class Outer<TOuter>
{
   public class Inner : Base<int>
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var innerType = GetTypeSymbol(compilation, "Test.Outer`1+Inner");

      var result = innerType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().NotBeNull();
   }
}
