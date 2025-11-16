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
      : base(output, 19_000)
   {
   }

   [Fact]
   public async Task Should_generate_complex_class_with_nullable_members()
   {
      var source = """

         using System;
         using Thinktecture;
         using System.Threading.Tasks;

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
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  [typeof(ComplexValueObjectAttribute).Assembly],
                                                                  ["No defining declaration found for implementing declaration of partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?, ref string?, ref Func<string?, Task<string?>?>?)'"]);

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
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  [typeof(ComplexValueObjectAttribute).Assembly],
                                                                  [
                                                                     "No defining declaration found for implementing declaration of partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?)'",
                                                                     "Partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?)' must have accessibility modifiers because it has a non-void return type."
                                                                  ]);

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
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  [typeof(ComplexValueObjectAttribute).Assembly],
                                                                  [
                                                                     "No defining declaration found for implementing declaration of partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?)'",
                                                                     "Partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?)' must have accessibility modifiers because it has a non-void return type."
                                                                  ]);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_code_for_keyed_class_with_int_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>]
         	public partial class GenericValueObject<T>
               where T : IEquatable<T>
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.GenericValueObject`1.ValueObject.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Formattable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Parsable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Comparable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.AdditionOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_code_for_keyed_struct_with_int_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>]
         	public partial struct GenericValueObject<T>
               where T : IEquatable<T>
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.GenericValueObject`1.ValueObject.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Formattable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Parsable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.Comparable.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.AdditionOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.GenericValueObject`1.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_code_for_keyed_class_with_string_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<string>]
         	public partial class GenericStringValueObject<T>
               where T : class
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.GenericStringValueObject`1.ValueObject.g.cs",
                        "Thinktecture.Tests.GenericStringValueObject`1.Parsable.g.cs",
                        "Thinktecture.Tests.GenericStringValueObject`1.Comparable.g.cs",
                        "Thinktecture.Tests.GenericStringValueObject`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.GenericStringValueObject`1.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_code_for_keyed_class_with_guid_and_multiple_type_parameters()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<Guid>]
         	public partial class MultiGenericValueObject<T1, T2>
               where T1 : class
               where T2 : struct
         	{
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.MultiGenericValueObject`2.ValueObject.g.cs",
                        "Thinktecture.Tests.MultiGenericValueObject`2.Formattable.g.cs",
                        "Thinktecture.Tests.MultiGenericValueObject`2.Parsable.g.cs",
                        "Thinktecture.Tests.MultiGenericValueObject`2.Comparable.g.cs",
                        "Thinktecture.Tests.MultiGenericValueObject`2.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.MultiGenericValueObject`2.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_code_for_complex_class_with_generic()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            public partial class ComplexValueObject<T1, T2, T3>
               where T1 : class, IEquatable<T1>
               where T3 : struct
            {
               public T1 Property { get; }
               public T2? NullableProperty { get; }
               public T3 StructProperty { get; }
               public T3? NullableStructProperty { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);
      await VerifyAsync(output);
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_equals_method_if_SkipEqualityComparison_is_true()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject(SkipEqualityComparison = true)]
           public partial struct TestValueObject
           {
           }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_not_generate_equals_method_on_keyed_value_object_if_SkipEqualityComparison_is_true()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ValueObject<int>(SkipEqualityComparison = true)]
           public partial struct TestValueObject
           {
           }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
           [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
           [KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
         	public partial class TestValueObject
         	{
         	}
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
           [KeyMemberEqualityComparer<ComparerAccessors.Default<Foo>, Foo>]
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               [MemberEqualityComparer<ComparerAccessors.Default<int>, int>]
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
   public async Task Should_generate_complex_class_with_8_members_and_ObjectFactoryAttribute()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
           [ObjectFactory<string>]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               [MemberEqualityComparer<ComparerAccessors.Default<int>, int>]
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
                        "Thinktecture.Tests.TestValueObject.ComplexValueObject.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_class_with_8_members_and_ObjectFactoryAttribute_and_UseForSerialization()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
           [ComplexValueObject]
           [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public readonly string _stringValue;

               [MemberEqualityComparer<ComparerAccessors.Default<int>, int>]
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
                        "Thinktecture.Tests.TestValueObject.ComplexValueObject.g.cs");
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
                        "Thinktecture.Tests._1TestValueObject.ValueObject.g.cs",
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

   [Fact]
   public async Task Should_handle_explicitly_implemented_IParsable()
   {
      var source = """

         using System;
         using Thinktecture;
         using System.Diagnostics.CodeAnalysis;

         #nullable enable

         namespace Thinktecture.Tests;

         public struct StructWithExplicitInterfaceImplementation : IParsable<StructWithExplicitInterfaceImplementation>
         {
            static StructWithExplicitInterfaceImplementation IParsable<StructWithExplicitInterfaceImplementation>.Parse(
               string s,
               IFormatProvider? provider)
            {
               throw new NotImplementedException();
            }

            static bool IParsable<StructWithExplicitInterfaceImplementation>.TryParse(
               [NotNullWhen(true)] string? s,
               IFormatProvider? provider,
               out StructWithExplicitInterfaceImplementation result)
            {
               throw new NotImplementedException();
            }
         }

         [ValueObject<StructWithExplicitInterfaceImplementation>]
         public partial class ValueObjectForStructWithExplicitInterfaceImplementation;

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.ValueObject.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.Parsable.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_handle_explicitly_implemented_IFormattable()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests;

         public struct StructWithExplicitInterfaceImplementation : IFormattable
         {
            string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
            {
               throw new NotImplementedException();
            }
         }

         [ValueObject<StructWithExplicitInterfaceImplementation>]
         public partial class ValueObjectForStructWithExplicitInterfaceImplementation;

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.ValueObject.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.Formattable.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_handle_explicitly_implemented_IComparable()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests;

         public struct StructWithExplicitInterfaceImplementation : IComparable, IComparable<StructWithExplicitInterfaceImplementation>
         {
            int IComparable.CompareTo(object? obj)
            {
               throw new NotImplementedException();
            }

            int IComparable<StructWithExplicitInterfaceImplementation>.CompareTo(StructWithExplicitInterfaceImplementation other)
            {
               throw new NotImplementedException();
            }
         }

         [ValueObject<StructWithExplicitInterfaceImplementation>]
         public partial class ValueObjectForStructWithExplicitInterfaceImplementation;

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.ValueObject.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.Comparable.g.cs",
                        "Thinktecture.Tests.ValueObjectForStructWithExplicitInterfaceImplementation.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public void Should_not_generate_code_for_keyed_class_with_nullable_key_type_annotation()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<string?>]
         	public partial class TestValueObject
         	{
           }
         }

         """;

      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(
         source,
         [typeof(ComplexValueObjectAttribute).Assembly],
         [
            "Type 'string' cannot be used in this context because it cannot be represented in metadata.",
            "A key member must not be nullable"
         ]);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_code_for_keyed_class_with_nullable_struct_key_type()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int?>]
         	public partial class TestValueObject
         	{
           }
         }

         """;

      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(
         source,
         [typeof(ComplexValueObjectAttribute).Assembly],
         ["A key member must not be nullable"]);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_code_for_keyed_class_nested_in_generic_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            public class Container<T>
            {
               [ValueObject<int>]
         	   public partial class TestValueObject
         	   {
               }
            }
         }

         """;

      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(
         source,
         [typeof(ComplexValueObjectAttribute).Assembly],
         ["Type 'TestValueObject' must not be inside a generic type"]);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_code_for_complex_class_nested_in_generic_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            public class Container<T>
            {
               [ComplexValueObject]
         	   public partial class TestValueObject
         	   {
               }
            }
         }

         """;

      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(
         source,
         [typeof(ComplexValueObjectAttribute).Assembly],
         ["Type 'TestValueObject' must not be inside a generic type"]);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Guid_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<Guid>]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_DateTime_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<DateTime>]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_decimal_based_keyed_class()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<decimal>]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_keyed_class_with_SkipKeyMember_and_custom_property()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SkipKeyMember = true, KeyMemberName = "Value")]
         	public partial class TestValueObject
         	{
               public int Value { get; private init; }
           }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_keyed_class_with_SkipKeyMember_and_custom_field()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<string>(SkipKeyMember = true, KeyMemberName = "_customValue")]
         	public partial class TestValueObject
         	{
               private readonly string _customValue;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_complex_class_with_required_members()
   {
      // This will cause TT-Analyzer to raise a compiler error, but the analyzer is not run for generated code.

      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
               public required string Property1 { get; init; }
               public required int Property2 { get; init; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_init_only_properties()
   {
      // This will cause TT-Analyzer to raise a compiler error, but the analyzer is not run for generated code.

      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
               public string Property1 { get; init; }
               public int Property2 { get; init; }
           }
         }

         """;

      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_multiple_ignored_members()
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
               public string Property1 { get; }

               [IgnoreMember]
               public string IgnoredProperty1 { get; }

               public int Property2 { get; }

               [IgnoreMember]
               public int IgnoredProperty2 { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_different_member_equality_comparers()
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
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public string Property1 { get; }

               [MemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
               public string Property2 { get; }

               [MemberEqualityComparer<ComparerAccessors.Default<int>, int>]
               public int Property3 { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_complex_class_with_ValidationError_in_ValidateFactoryArguments()
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
               public string Property { get; }

               static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string property)
               {
                  if (string.IsNullOrWhiteSpace(property))
                     validationError = new ValidationError("Property cannot be empty");
               }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  [typeof(ComplexValueObjectAttribute).Assembly],
                                                                  ["No defining declaration found for implementing declaration of partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?, ref string)'"]);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_SerializationFrameworks_None()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_complex_class_with_SerializationFrameworks_None()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(SerializationFrameworks = SerializationFrameworks.None)]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_all_arithmetic_operators_set_to_None()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(AdditionOperators = OperatorsGeneration.None,
                             SubtractionOperators = OperatorsGeneration.None,
                             MultiplyOperators = OperatorsGeneration.None,
                             DivisionOperators = OperatorsGeneration.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_only_AdditionOperators()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SubtractionOperators = OperatorsGeneration.None,
                             MultiplyOperators = OperatorsGeneration.None,
                             DivisionOperators = OperatorsGeneration.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_mixed_arithmetic_operators()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<decimal>(AdditionOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads,
                                 SubtractionOperators = OperatorsGeneration.None,
                                 MultiplyOperators = OperatorsGeneration.Default,
                                 DivisionOperators = OperatorsGeneration.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_struct_with_AllowDefaultStructs_false()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(AllowDefaultStructs = false)]
         	public partial struct TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_complex_struct_with_AllowDefaultStructs_false()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(AllowDefaultStructs = false)]
         	public partial struct TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_SkipIComparable()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SkipIComparable = true)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_SkipIParsable()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SkipIParsable = true)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_SkipIFormattable()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SkipIFormattable = true)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_keyed_class_with_SkipToString()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SkipToString = true)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_complex_class_with_SkipToString()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(SkipToString = true)]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_ComparisonOperators_None()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(ComparisonOperators = OperatorsGeneration.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_EqualityComparisonOperators_None()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(EqualityComparisonOperators = OperatorsGeneration.None)]
         	public partial class TestValueObject
         	{
            }
         }

         """;

      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
                        "Thinktecture.Tests.TestValueObject.Formattable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Comparable.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.AdditionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.SubtractionOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.MultiplyOperators.g.cs",
                        "Thinktecture.Tests.TestValueObject.DivisionOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_custom_factory_method_names()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(CreateFactoryMethodName = "New",
                              TryCreateFactoryMethodName = "TryNew")]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_complex_class_with_custom_factory_method_names()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(CreateFactoryMethodName = "Build",
                                TryCreateFactoryMethodName = "TryBuild")]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_public_constructor()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(ConstructorAccessModifier = AccessModifier.Public)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_complex_class_with_SkipFactoryMethods_and_ObjectFactory()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(SkipFactoryMethods = true)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject
         	{
               public string Property { get; }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ComplexValueObject.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_class_with_long_key()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<long>]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
   public async Task Should_generate_keyed_class_with_double_key()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ValueObject<double>]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ValueObjectSourceGenerator>(source, typeof(ComplexValueObjectAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ValueObject.g.cs",
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
}
