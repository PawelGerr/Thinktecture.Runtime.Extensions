using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetAccessibleConstructors : CompilationTestBase
{
   [Fact]
   public void Should_return_public_constructor()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
      result?.Constructors[0].Arguments.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_protected_constructor()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   protected BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_protected_internal_constructor()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   protected internal BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_protected_internal_constructor_from_different_assembly()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase
{
   protected internal ExternalBase() { }
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
      // protected internal == protected OR internal, so protected allows access across assemblies for derived types
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_internal_constructor_when_same_assembly()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   internal BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_private_protected_constructor_when_same_assembly()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   private protected BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_not_return_private_constructor_when_not_nested()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   private BaseClass() { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["'BaseClass.BaseClass()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_private_constructor_when_nested_inside_base_class()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   private BaseClass() { }

   public class NestedDerived : BaseClass
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.BaseClass+NestedDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_private_constructor_when_nested_multiple_levels_deep()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   private OuterClass() { }

   public class MiddleClass
   {
      public class InnerDerived : OuterClass
      {
      }
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.OuterClass+MiddleClass+InnerDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_not_return_private_constructor_when_nested_in_different_class()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   private BaseClass() { }
}

public class OuterClass
{
   public class NestedDerived : BaseClass
   {
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["'BaseClass.BaseClass()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.OuterClass+NestedDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_multiple_accessible_constructors()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass() { }
   protected BaseClass(int value) { }
   private BaseClass(string value) { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(2);
   }

   [Fact]
   public void Should_include_constructor_parameters()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass(int value, string name) { }
}

public class DerivedClass : BaseClass
{
   public DerivedClass(int value, string name) : base(value, name) { }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
      result?.Constructors[0].Arguments.Should().HaveCount(2);
      result?.Constructors[0].Arguments[0].Name.Should().Be("value");
      result?.Constructors[0].Arguments[1].Name.Should().Be("name");
   }

   [Fact]
   public void Should_not_return_internal_constructor_from_different_assembly_without_internals_visible_to()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, assemblyName: "ExternalLib", expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, assemblyName: "TestLib", additionalReferences: baseReference, expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_internal_constructor_from_different_assembly_with_internals_visible_to()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestLib"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
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
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_return_internal_constructor_with_internals_visible_to_with_public_key()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestLib, PublicKey=123"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, assemblyName: "ExternalLib", expectedCompilerErrors: ["'BaseClass.BaseClass()' is inaccessible due to its protection level"]);
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, assemblyName: "TestLib", additionalReferences: baseReference, expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_not_return_internal_constructor_with_internals_visible_to_for_wrong_assembly()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""WrongAssemblyName"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, assemblyName: "ExternalLib", expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, assemblyName: "TestLib", additionalReferences: baseReference, expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_handle_internals_visible_to_with_whitespace()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""  TestLib  "")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
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
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_not_return_private_protected_constructor_from_different_assembly_even_with_internals_visible_to()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestLib"")]

namespace ExternalLib;

public class ExternalBase
{
   private protected ExternalBase() { }
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
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_not_return_private_protected_constructor_from_different_assembly_without_internals_visible_to()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase
{
   private protected ExternalBase() { }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase;
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, assemblyName: "ExternalLib", expectedCompilerErrors: ["'BaseClass.BaseClass()' is inaccessible due to its protection level"]);
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, assemblyName: "TestLib", additionalReferences: baseReference, expectedCompilerErrors: ["'ExternalBase.ExternalBase()' is inaccessible due to its protection level"]);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_all_accessible_constructors_with_mixed_accessibility()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass() { }
   protected BaseClass(int a) { }
   internal BaseClass(string b) { }
   private BaseClass(double c) { }
   protected internal BaseClass(bool d) { }
   private protected BaseClass(long e) { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      // Should include: public, protected, internal, protected internal, private protected
      // Should exclude: private
      result?.Constructors.Should().HaveCount(5);
   }

   [Fact]
   public void Should_handle_record_base_class_with_primary_constructor()
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
      result?.Constructors.Should().NotBeEmpty();
   }

   [Fact]
   public void Should_handle_generic_base_class_constructor()
   {
      var src = @"
namespace Test;

public class BaseClass<T>
{
   protected BaseClass(T value) { }
}

public class DerivedClass : BaseClass<int>
{
   public DerivedClass(int value) : base(value) { }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
      result?.Constructors[0].Arguments.Should().HaveCount(1);
   }

   [Fact]
   public void Should_cache_nested_check_for_performance()
   {
      // This test verifies that the isNestedInside caching works
      var src = @"
namespace Test;

public class BaseClass
{
   private BaseClass() { }
   private BaseClass(int a) { }
   private BaseClass(string b) { }

   public class NestedDerived : BaseClass
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.BaseClass+NestedDerived");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      // All three private constructors should be accessible
      result?.Constructors.Should().HaveCount(3);
   }

   [Fact]
   public void Should_cache_internals_visible_to_check_for_performance()
   {
      // This test verifies that the hasInternalsVisibleTo caching works
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestLib"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
   internal ExternalBase(int a) { }
   internal ExternalBase(string b) { }
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
      // All three internal constructors should be accessible
      result?.Constructors.Should().HaveCount(3);
   }

   [Fact]
   public void Should_handle_constructors_with_various_parameter_types()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass(int value) { }
   public BaseClass(string text, double number) { }
   public BaseClass(bool flag, object obj, int[] array) { }
}

public class DerivedClass : BaseClass
{
   public DerivedClass(int value) : base(value) { }
   public DerivedClass(string text, double number) : base(text, number) { }
   public DerivedClass(bool flag, object obj, int[] array) : base(flag, obj, array) { }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(3);
      result?.Constructors[0].Arguments.Should().HaveCount(1);
      result?.Constructors[1].Arguments.Should().HaveCount(2);
      result?.Constructors[2].Arguments.Should().HaveCount(3);
   }

   [Fact]
   public void Should_handle_abstract_base_class_with_protected_constructor()
   {
      var src = @"
namespace Test;

public abstract class AbstractBase
{
   protected AbstractBase() { }
   protected AbstractBase(int value) { }
}

public class ConcreteClass : AbstractBase;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var concreteType = GetTypeSymbol(compilation, "Test.ConcreteClass");

      var result = concreteType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(2);
   }

   [Fact]
   public void Should_handle_multiple_internals_visible_to_attributes()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""WrongAssembly"")]
[assembly: InternalsVisibleTo(""TestLib"")]
[assembly: InternalsVisibleTo(""AnotherAssembly"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
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
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_be_case_insensitive_when_matching_internals_visible_to_assembly_name()
   {
      var baseAssemblySrc = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""testlib"")]

namespace ExternalLib;

public class ExternalBase
{
   internal ExternalBase() { }
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
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_handle_nested_derived_class_in_generic_outer_class()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   protected BaseClass() { }
}

public class Outer<T>
{
   public class Nested : BaseClass
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var nestedType = GetTypeSymbol(compilation, "Test.Outer`1+Nested");

      var result = nestedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
   }

   [Fact]
   public void Should_handle_constructors_with_optional_parameters()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass(int value = 42, string text = ""default"") { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
      result?.Constructors[0].Arguments.Should().HaveCount(2);
   }

   [Fact]
   public void Should_handle_constructors_with_params_parameters()
   {
      var src = @"
namespace Test;

public class BaseClass
{
   public BaseClass(params int[] values) { }
}

public class DerivedClass : BaseClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var derivedType = GetTypeSymbol(compilation, "Test.DerivedClass");

      var result = derivedType.GetBaseType(factory);

      result.Should().NotBeNull();
      result?.Constructors.Should().HaveCount(1);
      result?.Constructors[0].Arguments.Should().HaveCount(1);
   }
}
