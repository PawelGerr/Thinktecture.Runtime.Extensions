using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class GetAssignableFieldsAndPropertiesAndCheckForReadOnly : CompilationTestBase
{
   // Basic field tests
   [Fact]
   public void Returns_readonly_fields()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field1;
   public readonly string _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().AllSatisfy(m => m.Kind.Should().Be(SymbolKind.Field));
   }

   [Fact]
   public void Excludes_const_fields()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public const int ConstField = 42;
   private const string PrivateConstField = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: false, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Excludes_static_fields_when_instance_members_only()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _instanceField;
   public static readonly string StaticField = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_instanceField");
   }

   [Fact]
   public void Includes_static_fields_when_instance_members_only_is_false()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _instanceField;
   public static readonly string StaticField = ""test"";
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: false, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().Contain(m => m.Name == "_instanceField");
      result.Should().Contain(m => m.Name == "StaticField");
   }

   [Fact]
   public void Excludes_fields_with_ignore_member_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class TestClass
{
   private readonly int _field1;

   [IgnoreMember]
   private readonly string _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_field1");
   }

   [Fact]
   public void Excludes_backing_fields_for_auto_properties()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should return the property, not the backing field
      result.Should().HaveCount(1);
      result[0].Kind.Should().Be(SymbolKind.Property);
      result[0].Name.Should().Be("Name");
   }

   // Basic property tests
   [Fact]
   public void Returns_readonly_property_with_only_getter()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; }
   public int Age { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().AllSatisfy(m => m.Kind.Should().Be(SymbolKind.Property));
   }

   [Fact]
   public void Returns_property_with_private_init_accessor()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; private init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Returns_property_with_private_accessibility_and_init_accessor()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private string Name { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Excludes_properties_with_ignore_member_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class TestClass
{
   public string Name { get; }

   [IgnoreMember]
   public int Age { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Excludes_static_properties_when_instance_members_only()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; }
   public static int Count { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Includes_static_properties_when_instance_members_only_is_false()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; }
   public static int Count { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: false, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().Contain(m => m.Name == "Name");
      result.Should().Contain(m => m.Name == "Count");
   }

   [Fact]
   public void Excludes_expression_bodied_property()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public int Value => 42;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Includes_property_with_field_keyword_in_expression_body()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get => field; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Includes_property_with_field_keyword_in_accessor_body()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get { return field; } }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Excludes_expression_bodied_property_returning_field()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _value;

   public int Value => _value;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should only return the field, not the computed property
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_value");
   }

   [Fact]
   public void Excludes_property_with_custom_getter_implementation()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _value;

   public int Value { get { return _value * 2; } }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should only return the field, not the computed property
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_value");
   }

   [Fact]
   public void Excludes_property_with_custom_getter_expression()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _value;

   public int Value { get => _value + 10; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should only return the field, not the computed property
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_value");
   }

   // Mixed field and property tests
   [Fact]
   public void Returns_both_fields_and_properties()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
   public string Name { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().Contain(m => m.Name == "_field" && m.Kind == SymbolKind.Field);
      result.Should().Contain(m => m.Name == "Name" && m.Kind == SymbolKind.Property);
   }

   // Property accessor variations
   [Fact]
   public void Includes_property_with_default_getter_and_private_init()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; private init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Includes_property_with_field_keyword_and_private_init()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get => field; private init => field = value; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   // Edge cases
   [Fact]
   public void Returns_empty_for_class_with_no_fields_or_properties()
   {
      var src = @"
namespace Test;

public class TestClass;
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Excludes_write_only_properties()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private int _value;

   public int Value { set => _value = value; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should return the backing field only, not the write-only property
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_value");
   }

   [Fact]
   public void Handles_properties_from_external_assembly()
   {
      var baseAssemblySrc = @"
namespace ExternalLib;

public class ExternalBase
{
   public string Name { get; }
}
";
      var derivedAssemblySrc = @"
namespace Test;

public class DerivedClass : ExternalLib.ExternalBase
{
   public int Age { get; }
}
";
      var baseCompilation = CreateCompilation(baseAssemblySrc, "ExternalLib");
      var baseReference = baseCompilation.ToMetadataReference();
      var derivedCompilation = CreateCompilation(derivedAssemblySrc, "TestLib", additionalReferences: baseReference);
      var factory = TypedMemberStateFactory.Create(derivedCompilation);
      var derivedType = GetTypeSymbol(derivedCompilation, "Test.DerivedClass");

      var result = derivedType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should only return members from the derived class, not base class
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Age");
   }

   [Fact]
   public void Excludes_indexers()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int[] _values = new int[10];

   public int this[int index]
   {
      get => _values[index];
   }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Should only return the field, indexers are not included
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("_values");
   }

   [Fact]
   public void Handles_required_properties()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public required string Name { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Handles_required_fields()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public required string Name;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Name");
   }

   [Fact]
   public void Handles_record_positional_properties()
   {
      var src = @"
namespace Test;

public record TestRecord
{
   public string Name { get; init; }
   public int Age { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestRecord");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().Contain(m => m.Name == "Name");
      result.Should().Contain(m => m.Name == "Age");
   }

   [Fact]
   public void Handles_struct_with_fields_and_properties()
   {
      var src = @"
namespace Test;

public struct TestStruct
{
   private readonly int _field;
   public string Name { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestStruct");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().Contain(m => m.Name == "_field");
      result.Should().Contain(m => m.Name == "Name");
   }

   [Fact]
   public void Handles_record_struct_positional_properties()
   {
      var src = @"
namespace Test;

public record struct TestRecordStruct
{
   public string Name { get; init; }
   public int Age { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestRecordStruct");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(2);
      result.Should().Contain(m => m.Name == "Name");
      result.Should().Contain(m => m.Name == "Age");
   }

   [Fact]
   public void Handles_multiple_accessibility_levels()
   {
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _private;
   protected readonly int _protected;
   internal readonly int _internal;
   public readonly int _public;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      result.Should().HaveCount(4);
   }

   [Fact]
   public void Excludes_compiler_generated_members()
   {
      var src = @"
namespace Test;

public class TestClass
{
   public event System.EventHandler? MyEvent;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");

      var result = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, instanceMembersOnly: true, populateValueObjectMemberSettings: false, cancellationToken: TestContext.Current.CancellationToken).ToList();

      // Event backing fields should be excluded (CanBeReferencedByName check)
      result.Should().BeEmpty();
   }
}
