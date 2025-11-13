namespace Thinktecture.Runtime.Tests.TypeDeclarationSyntaxExtensionsTests;

public class IsGeneric : CompilationTestBase
{
   [Fact]
   public void Returns_false_for_non_generic_class()
   {
      var src = @"
namespace Test;

public class MyClass
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("<T>")]
   [InlineData("<T1, T2>")]
   [InlineData("<T1, T2, T3, T4>")]
   public void Returns_true_for_generic_class_with_type_parameters(string typeParams)
   {
      var src = $@"
namespace Test;

public class MyClass{typeParams}
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyStruct");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyStruct");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_record()
   {
      var src = @"
namespace Test;

public record MyRecord
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyRecord");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_record()
   {
      var src = @"
namespace Test;

public record MyRecord<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyRecord");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_record_struct()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyRecordStruct");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_record_struct()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyRecordStruct");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_generic_interface_with_multiple_type_parameters()
   {
      var src = @"
namespace Test;

public interface IMyInterface<TKey, TValue>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_nested_non_generic_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class InnerClass
   {
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "InnerClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_nested_generic_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class InnerClass<T>
   {
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "InnerClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_class_nested_in_generic_class()
   {
      var src = @"
namespace Test;

public class OuterClass<T>
{
   public class InnerClass
   {
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "InnerClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_nested_in_generic_class()
   {
      var src = @"
namespace Test;

public class OuterClass<T>
{
   public class InnerClass<U>
   {
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "InnerClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_constrained_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass<T> where T : class
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_multiple_constraints()
   {
      var src = @"
namespace Test;

public class MyClass<T> where T : class, new()
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_multiple_type_parameters_and_constraints()
   {
      var src = @"
namespace Test;

public class MyClass<TKey, TValue>
   where TKey : notnull
   where TValue : class, new()
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData("partial", "class")]
   [InlineData("abstract", "class")]
   [InlineData("sealed", "class")]
   [InlineData("static", "class")]
   [InlineData("readonly", "struct")]
   [InlineData("ref", "struct")]
   public void Returns_false_for_non_generic_types_with_modifiers(string modifier, string typeKind)
   {
      var typeName = typeKind == "class" ? "MyClass" : typeKind == "struct" ? "MyStruct" : "MyRefStruct";
      var src = $@"
namespace Test;

public {modifier} {typeKind} {typeName}
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, typeName);

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("partial", "class")]
   [InlineData("abstract", "class")]
   [InlineData("sealed", "class")]
   [InlineData("readonly", "struct")]
   [InlineData("ref", "struct")]
   public void Returns_true_for_generic_types_with_modifiers(string modifier, string typeKind)
   {
      var typeName = typeKind == "class" ? "MyClass" : typeKind == "struct" ? "MyStruct" : "MyRefStruct";
      var src = $@"
namespace Test;

public {modifier} {typeKind} {typeName}<T>
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, typeName);

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   [InlineData("record")]
   [InlineData("record struct")]
   [InlineData("record class")]
   [InlineData("interface")]
   public void Returns_false_for_non_generic_type_kinds(string typeKind)
   {
      var src = $@"
namespace Test;

public {typeKind} MyType
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyType");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   [InlineData("record")]
   [InlineData("record struct")]
   [InlineData("record class")]
   [InlineData("interface")]
   public void Returns_true_for_generic_type_kinds(string typeKind)
   {
      var src = $@"
namespace Test;

public {typeKind} MyType<T>
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyType");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_interface_with_covariant_type_parameter()
   {
      var src = @"
namespace Test;

public interface IMyInterface<out T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_interface_with_contravariant_type_parameter()
   {
      var src = @"
namespace Test;

public interface IMyInterface<in T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_interface_with_mixed_variance_type_parameters()
   {
      var src = @"
namespace Test;

public interface IMyInterface<out TResult, in TInput>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "IMyInterface");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_class_with_base_class()
   {
      var src = @"
namespace Test;

public class BaseClass { }

public class DerivedClass : BaseClass
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "DerivedClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_with_base_class()
   {
      var src = @"
namespace Test;

public class BaseClass { }

public class DerivedClass<T> : BaseClass
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "DerivedClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_class_implementing_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface { }

public class MyClass : IMyInterface
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_implementing_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface { }

public class MyClass<T> : IMyInterface
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_class_with_generic_base_class()
   {
      var src = @"
namespace Test;

public class BaseClass<T> { }

public class DerivedClass : BaseClass<int>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "DerivedClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_with_generic_base_class()
   {
      var src = @"
namespace Test;

public class BaseClass<T> { }

public class DerivedClass<T> : BaseClass<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "DerivedClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_correct_result_for_multiple_types_in_same_file()
   {
      var src = @"
namespace Test;

public class NonGenericClass
{
}

public class GenericClass<T>
{
}

public class AnotherNonGenericClass
{
}
";
      var nonGeneric1 = GetTypeDeclaration(src, "NonGenericClass");
      var generic = GetTypeDeclaration(src, "GenericClass");
      var nonGeneric2 = GetTypeDeclaration(src, "AnotherNonGenericClass");

      nonGeneric1.IsGeneric().Should().BeFalse();
      generic.IsGeneric().Should().BeTrue();
      nonGeneric2.IsGeneric().Should().BeFalse();
   }

   [Theory]
   [InlineData(false, "namespace Test;\n\npublic class MyClass\n{\n}\n")]
   [InlineData(true, "namespace Test;\n\npublic class MyClass<T>\n{\n}\n")]
   [InlineData(false, "namespace Test\n{\n   public class MyClass\n   {\n   }\n}\n")]
   [InlineData(true, "namespace Test\n{\n   public class MyClass<T>\n   {\n   }\n}\n")]
   [InlineData(false, "public class MyClass\n{\n}\n")]
   [InlineData(true, "public class MyClass<T>\n{\n}\n")]
   public void Returns_correct_result_for_classes_in_various_namespace_styles(bool expectedIsGeneric, string classCode)
   {
      var typeDeclaration = GetTypeDeclaration(classCode, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().Be(expectedIsGeneric);
   }

   [Fact]
   public void Returns_false_for_non_generic_class_with_generic_method()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public void GenericMethod<T>(T value) { }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_having_struct_constraint()
   {
      var src = @"
namespace Test;

public class MyClass<T> where T : struct
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_having_unmanaged_constraint()
   {
      var src = @"
namespace Test;

public class MyClass<T> where T : unmanaged
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_having_base_class_constraint()
   {
      var src = @"
namespace Test;

public class BaseClass { }

public class MyClass<T> where T : BaseClass
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_having_interface_constraint()
   {
      var src = @"
namespace Test;

public interface IMyInterface { }

public class MyClass<T> where T : IMyInterface
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_having_multiple_interface_constraints()
   {
      var src = @"
using System;
namespace Test;

public class MyClass<T> where T : IDisposable, IComparable<T>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_with_type_parameter_referencing_other_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass<T, U> where U : T
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsGeneric();

      result.Should().BeTrue();
   }
}
