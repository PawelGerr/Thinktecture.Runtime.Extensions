using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class ValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public ValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public async Task Should_generate_complex_class_with_nullable_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject
         	{
               public string? Prop1 { get; }
               public Func<string?, Task<string?>?>? Prop2 { get; }

               static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string? prop1, ref Func<string?, Task<string?>?>? prop2)
               {
               }
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_post_init_method_if_validation_method_returns_struct()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject
         	{
               static partial int ValidateFactoryArguments(ref ValidationError? validationError)
               {
                  return 42;
               }
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_post_init_method_if_validation_method_returns_nullable_string()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject
         	{
               static partial string? ValidateFactoryArguments(ref ValidationError? validationError)
               {
                  return String.Empty;
               }
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_code_for_keyed_class_with_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>]
         	public partial class TestValueObject<T>
         	{
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_code_for_complex_class_with_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject<T>
         	{
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_complex_class_without_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         [ComplexValueObject]
         public partial class TestValueObject
         {
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_without_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_not_generate_factory_methods_if_SkipFactoryMethods_is_true()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(SkipFactoryMethods = true)]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_struct_without_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial struct TestValueObject
         	{
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_struct_with_custom_default_instance_property_name()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject(DefaultInstancePropertyName = "Null",
                               AllowDefaultStructs = true)]
         	public partial struct TestValueObject
         	{
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_struct()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>]
         	public partial struct TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_keyed_struct()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>]
         	public partial struct TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_keyed_struct_custom_default_instance_property_name()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(DefaultInstancePropertyName = "Null",
                             AllowDefaultStructs = true)]
         	public partial struct TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_keyed_struct_with_custom_int_key_member_with_init_only()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(SkipKeyMember = true)]
         	public partial struct TestValueObject
         	{
               public readonly int _value { get; private init; }
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_struct_with_NullInFactoryMethodsYieldsNull_which_should_be_ignored()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>(NullInFactoryMethodsYieldsNull = true)]
         	public partial struct TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_struct_with_EmptyStringInFactoryMethodsYieldsNull_which_should_be_ignored()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
         	public partial struct TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_DateOnly_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<DateOnly>(KeyMemberName = "_value")]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_DateOnly_based_keyed_class_with_DefaultWithKeyTypeOverloads()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<DateOnly>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_keyed_class_with_DefaultWithKeyTypeOverloads()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      SubtractionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      MultiplyOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                      DivisionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
         	public partial class TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_class_with_NullInFactoryMethodsYieldsNull()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>(NullInFactoryMethodsYieldsNull = true)]
         	public partial class TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_class_with_EmptyStringInFactoryMethodsYieldsNull()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
         	public partial class TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_class_with_additional_member()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>]
         	public partial class TestValueObject
         	{
               public readonly string OtherField;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_keyed_class_with_custom_comparers()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>]
           [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
           [ValueObjectKeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
         	public partial class TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_non_IEquatable_key_member_but_with_custom_equality_comparer()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<Foo>]
           [ValueObjectKeyMemberEqualityComparer<ComparerAccessors.Default<Foo>, Foo>]
         	public partial class TestValueObject
         	{
           }

           public class Foo
           {
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Theory]
   [InlineData(ConversionOperatorsGeneration.None)]
   [InlineData(ConversionOperatorsGeneration.Implicit)]
   [InlineData(ConversionOperatorsGeneration.Explicit)]
   public async Task Should_change_conversion_from_key(
      ConversionOperatorsGeneration operatorsGeneration)
   {
      var source = $$"""
         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<string>(ConversionFromKeyMemberType = ConversionOperatorsGeneration.{{operatorsGeneration}})]
         	public partial class TestValueObject
         	{
         	}
         }
         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(operatorsGeneration.ToString(),
                        outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Theory]
   [InlineData(ConversionOperatorsGeneration.None)]
   [InlineData(ConversionOperatorsGeneration.Implicit)]
   [InlineData(ConversionOperatorsGeneration.Explicit)]
   public async Task Should_change_conversion_to_key(
      ConversionOperatorsGeneration operatorsGeneration)
   {
      var source = $$"""
         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<string>(ConversionToKeyMemberType = ConversionOperatorsGeneration.{{operatorsGeneration}})]
         	public partial class TestValueObject
         	{
         	}
         }
         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(operatorsGeneration.ToString(),
                        outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Theory]
   [InlineData(ConversionOperatorsGeneration.None)]
   [InlineData(ConversionOperatorsGeneration.Implicit)]
   [InlineData(ConversionOperatorsGeneration.Explicit)]
   public async Task Should_change_unsafe_conversion_to_key(
      ConversionOperatorsGeneration operatorsGeneration)
   {
      var source = $$"""
         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(UnsafeConversionToKeyMemberType = ConversionOperatorsGeneration.{{operatorsGeneration}})]
         	public partial class TestValueObject
         	{
         	}
         }
         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(operatorsGeneration.ToString(),
                        outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_class_with_8_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
         	public partial class TestValueObject
         	{
               [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               public readonly int _intValue;

               public string ReferenceProperty { get; }
               public string? NullableReferenceProperty { get; }
               public int StructProperty { get; }
               public int? NullableStructProperty { get; }

               public int ExpressionBodyProperty => 42;

               public int GetterExpressionProperty
               {
                  get => 42;
               }

               public int GetterBodyProperty
               {
                  get { return 42; }
               }

               public int SetterProperty
               {
                  set { }
               }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_8_members_and_ValueObjectFactoryAttribute()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
           [ValueObjectFactory<string>]
         	public partial class TestValueObject
         	{
               [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               public readonly int _intValue;

               public string ReferenceProperty { get; }
               public string? NullableReferenceProperty { get; }
               public int StructProperty { get; }
               public int? NullableStructProperty { get; }

               public int ExpressionBodyProperty => 42;

               public int GetterExpressionProperty
               {
                  get => 42;
               }

               public int GetterBodyProperty
               {
                  get { return 42; }
               }

               public int SetterProperty
               {
                  set { }
               }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_class_with_8_members_and_ValueObjectFactoryAttribute_and_UseForSerialization()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
           [ValueObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject
         	{
               [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public readonly string _stringValue;

               [ValueObjectMemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               public readonly int _intValue;

               public string ReferenceProperty { get; }
               public string? NullableReferenceProperty { get; }
               public int StructProperty { get; }
               public int? NullableStructProperty { get; }

               public int ExpressionBodyProperty => 42;

               public int GetterExpressionProperty
               {
                  get => 42;
               }

               public int GetterBodyProperty
               {
                  get { return 42; }
               }

               public int SetterProperty
               {
                  set { }
               }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_class_with_9_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
               public readonly string _value1;
               public readonly string _value2;
               public readonly string _value3;
               public readonly string _value4;
               public readonly string _value5;
               public readonly string _value6;
               public readonly string _value7;
               public readonly string _value8;
               public readonly string _value9;
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   public static readonly IEnumerable<object[]> StringComparisons = Enum.GetValues<StringComparison>()
                                                                        .Cast<object>()
                                                                        .Select(v => new[] { v });

   [Theory]
   [MemberData(nameof(StringComparisons))]
   public async Task Should_generate_complex_class_with_string_member(StringComparison stringComparison)
   {
      var source = $$"""

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(DefaultStringComparison = StringComparison.{{stringComparison}})]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(stringComparison.ToString(), output);
   }

   [Fact]
   public async Task Should_handle_special_chars()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(KeyMemberName = "_1Key")]
         	public partial class _1TestValueObject
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests._1TestValueObject.g.cs",
                        "Thinktecture.Tests._1TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests._1TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests._1TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests._1TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests._1TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests._1TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests._1TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests._1TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests._1TestValueObject.DivisionOperators.g.cs");
   }
}
