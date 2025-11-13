using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.MethodSymbolExtensionsTests;

public class IsUserDefinedComparisonOperator : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_valid_comparison_operator_with_same_types()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_inequality_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Inequality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_less_than_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator <(MyClass left, MyClass right) => false;
   public static bool operator >(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_LessThan").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_greater_than_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator <(MyClass left, MyClass right) => false;
   public static bool operator >(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_GreaterThan").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_less_than_or_equal_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator <=(MyClass left, MyClass right) => false;
   public static bool operator >=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_LessThanOrEqual").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_greater_than_or_equal_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator <=(MyClass left, MyClass right) => false;
   public static bool operator >=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_GreaterThanOrEqual").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_comparison_operator_with_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static bool operator ==(MyStruct left, MyStruct right) => true;
   public static bool operator !=(MyStruct left, MyStruct right) => false;
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var operatorMethod = structType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(structType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_operator_with_wrong_return_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int operator ==(MyClass left, MyClass right) => 0;
   public static int operator !=(MyClass left, MyClass right) => 0;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_operator_returning_nullable_bool()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool? operator ==(MyClass left, MyClass right) => null;
   public static bool? operator !=(MyClass left, MyClass right) => null;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_no_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool SomeMethod() => true;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("SomeMethod").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedComparisonOperator(classType);

      // Not a user-defined operator
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_unary_operator_with_one_parameter()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator true(MyClass obj) => true;
   public static bool operator false(MyClass obj) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_True").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_three_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass obj1, MyClass obj2, MyClass obj3) => true;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "Overloaded binary operator '==' takes two parameters",
         "The operator 'MyClass.operator ==(MyClass, MyClass, MyClass)' requires a matching operator '!=' to also be defined"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedComparisonOperator(classType);
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_operator_with_first_parameter_wrong_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(string left, MyClass right) => true;
   public static bool operator !=(string left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_operator_with_second_parameter_wrong_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, string right) => true;
   public static bool operator !=(MyClass left, string right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_operator_with_both_parameters_wrong_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(int left, string right) => true;
   public static bool operator !=(int left, string right) => false;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_comparison_operator_with_nullable_reference_types_matching()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass? left, MyClass? right) => true;
   public static bool operator !=(MyClass? left, MyClass? right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();
      // Get the nullable reference type from the first parameter
      var nullableType = operatorMethod.Parameters[0].Type;

      var result = operatorMethod.IsUserDefinedComparisonOperator(nullableType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_parameter_is_nullable_class_but_type_is_not()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass? left, MyClass? right) => true;
   public static bool operator !=(MyClass? left, MyClass? right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      // Test with non-nullable type when parameters are nullable
      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_type_is_nullable_class_but_parameters_are_not()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
}

public class AnotherClass
{
   public void Method(MyClass? param) { }
}
";
      var compilation = CreateCompilation(src);
      var anotherClassType = GetTypeSymbol(compilation, "Test.AnotherClass");
      var methodWithNullable = anotherClassType.GetMembers("Method").OfType<IMethodSymbol>().First();
      var nullableClassType = methodWithNullable.Parameters[0].Type;

      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(nullableClassType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_comparison_operator_with_nullable_value_type()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static bool operator ==(MyStruct? left, MyStruct? right) => true;
   public static bool operator !=(MyStruct? left, MyStruct? right) => false;
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var operatorMethod = structType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();
      var nullableStructType = operatorMethod.Parameters[0].Type;

      var result = operatorMethod.IsUserDefinedComparisonOperator(nullableStructType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_arithmetic_operator_that_returns_the_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass left, MyClass right) => left;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_regular_method_with_correct_signature()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool Compare(MyClass left, MyClass right) => true;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("Compare").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedComparisonOperator(classType);

      // Should return false - not a user-defined operator
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_comparison_operator_on_generic_type()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   public static bool operator ==(MyClass<T> left, MyClass<T> right) => true;
   public static bool operator !=(MyClass<T> left, MyClass<T> right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass`1");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_instance_method_with_correct_signature()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public bool Compare(MyClass left, MyClass right) => true;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("Compare").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedComparisonOperator(classType);

      result.Should().BeFalse(); // Not static and not a user-defined operator
   }

   [Fact]
   public void Returns_false_when_checking_against_different_type_than_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
}

public class OtherClass;
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var otherType = GetTypeSymbol(compilation, "Test.OtherClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(otherType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_comparison_operator_with_record_type()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   public static bool operator <(MyRecord left, MyRecord right) => false;
   public static bool operator >(MyRecord left, MyRecord right) => false;
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.MyRecord");
      var operatorMethod = recordType.GetMembers("op_LessThan").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(recordType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_parameters_match_exactly_including_nullability()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedComparisonOperator(classType);

      result.Should().BeTrue();
   }
}
