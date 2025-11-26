using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class SmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public SmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 48_000)
   {
   }

   [Fact]
   public async Task Should_generate_for_generic()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
            public partial class TestEnum<T>
         	{
               public static readonly TestEnum<T> Item1 = default!;
               public static readonly TestEnum<T> Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum`1.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum`1.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum`1.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum`1.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum`1.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public void Should_not_crash_if_type_flagged_with_multiple_source_gen_attributes()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            [SmartEnum<string>]
            [ValueObject<int>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(
         source,
         [typeof(ISmartEnum<>).Assembly],
         ["The type 'TestEnum' must not have more than one ValueObject/SmartEnum/Union-attribute"]);
      outputs.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_generate_keyless_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                      MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyless_class_with_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum]
            [ObjectFactory<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_having_ValidationErrorAttribute()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                              MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
           [ValidationError<TestEnumValidationError>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }

            public class TestEnumValidationError : IValidationError<TestEnumValidationError>
            {
               public string Message { get; }

               public TestEnumValidationError(string message)
               {
                  Message = message;
               }

               public static TestEnumValidationError Create(string message)
               {
                  return new TestEnumValidationError(message);
               }

               public override string ToString()
               {
                  return Message;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_without_Switch()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.None,
                               MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_without_Map()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
           [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                              MapMethods = SwitchMapMethodsGeneration.None)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_with_base_class_and_non_default_constructors()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public class BaseClass
            {
               protected BaseClass(int value)
               {
               }

               protected BaseClass(string key)
               {
               }
            }

           [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                              MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestEnum : BaseClass
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;

               private TestEnum(int value)
                  : base(value)
               {
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_without_namespace()
   {
      var source = """
         using System;
         using Thinktecture;

         [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                            MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "TestEnum.SmartEnum.g.cs",
                        "TestEnum.Comparable.g.cs",
                        "TestEnum.Parsable.g.cs",
                        "TestEnum.SpanParsable.g.cs",
                        "TestEnum.ComparisonOperators.g.cs",
                        "TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_string_based_class_with_inner_derived_type_which_is_generic()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
           [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                              MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item_1 = default!;
               public static readonly TestEnum Item_2 = default!;
               public static readonly TestEnum Item_int_1 = new GenericEnum<int>("GenericEnum<int> 1");
               public static readonly TestEnum Item_decimal_1 = new GenericEnum<decimal>("GenericEnum<decimal> 1");
               public static readonly TestEnum Item_decimal_2 = new GenericEnum<decimal>("GenericEnum<decimal> 2");
               public static readonly TestEnum Item_derived_1 = new DerivedEnum("DerivedEnum 1");
               public static readonly TestEnum Item_derived_2 = new DerivedEnum("DerivedEnum 2");

               private class GenericEnum<T> : TestEnum
               {
                  public GenericEnum(string key)
                  {
                  }
               }

               private class UnusedGenericEnum<T> : TestEnum
               {
                  public UnusedGenericEnum(string key)
                  {
                  }
               }

               private class DerivedEnum : TestEnum
               {
                  public DerivedEnum(string key)
                  {
                  }
               }

               private class UnusedDerivedEnum : TestEnum
               {
                  public UnusedDerivedEnum(string key)
                  {
                  }
               }

            }
         }

         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
           [SmartEnum<int>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_class_with_ComparisonOperators_DefaultWithKeyTypeOverloads()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
           [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_class_with_ObjectFactoryAttribute()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                            MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
            [ObjectFactory<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_int_based_class_with_ObjectFactoryAttribute_and_UseForSerialization()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                            MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_abstract_property()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;

               public abstract int Value { get; }

               private sealed class ConcreteEnum : TestEnum
               {
                  public override int Value => 100;

                  public ConcreteEnum(int key)
                  {
                  }
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_take_over_nullability_of_generic_members()
   {
      var source = """
         using System;
         using System.Threading.Tasks;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               public Func<string?, Task<string?>?>? Prop1 { get; }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
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

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>(ConversionFromKeyMemberType = ConversionOperatorsGeneration.{{operatorsGeneration}})]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(operatorsGeneration.ToString(),
                        outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
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

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>(ConversionToKeyMemberType = ConversionOperatorsGeneration.{{operatorsGeneration}})]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(operatorsGeneration.ToString(),
                        outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_of_type_func()
   {
      var source = """
         using System;
         using System.Threading.Tasks;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial Task<string?>? Method1(string? arg1);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_of_type_func_without_args()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               protected partial int Method1();
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_of_type_action()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               partial void Method1(string arg1, int arg2);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_of_type_action_without_args()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               internal partial void Method1();
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_ref_parameter()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial void Method1(ref int value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_in_parameter()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial string Method1(in int value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_ref_readonly_parameter()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial void Method1(ref readonly int value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_out_parameter()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial bool Method1(out int value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_mixed_ref_kinds()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial string Method1(string normal, ref int refValue, in double inValue, out bool outValue, ref readonly decimal readonlyValue);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_delegate_with_return_type_and_ref_parameters()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor]
               public partial int Method1(ref string value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_with_custom_name()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor(DelegateName = "CustomProcessDelegate")]
               public partial string Process(int value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_multiple_delegates_with_custom_names()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor(DelegateName = "StringProcessDelegate")]
               public partial string Process(string value);

               [UseDelegateFromConstructor(DelegateName = "IntProcessDelegate")]
               public partial int Process(int value);

               [UseDelegateFromConstructor(DelegateName = "BoolProcessDelegate")]
               public partial void Process(bool value);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_with_custom_name_and_complex_parameters()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor(DelegateName = "ComplexProcessDelegate")]
               public partial Dictionary<string, List<int>>? Process(Dictionary<int, string>? input, ref List<string>? refList, in HashSet<int> inSet, out Dictionary<string, object?>? outDict);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_with_custom_name_and_nullable_value_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
         	public abstract partial class TestEnum
         	{
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               [UseDelegateFromConstructor(DelegateName = "NullableValueTypeDelegate")]
               public partial int? Process(DateTime? dateTime, ref Guid? refGuid, in decimal? inDecimal, out bool? outBool);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_handle_special_chars()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>(KeyMemberName = "_1Key")]
            public partial class _1TestEnum
            {
               public static readonly _1TestEnum _1Item1 = null!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests._1TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests._1TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests._1TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests._1TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests._1TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests._1TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests._1TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_use_custom_SwitchMapStateParameterName()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>(SwitchMapStateParameterName = "context")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = null!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_guid_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<Guid>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_custom_ISpanParsable_key_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public readonly struct CustomSpanParsableKey : ISpanParsable<CustomSpanParsableKey>, IEquatable<CustomSpanParsableKey>
            {
               public int Value { get; }

               public CustomSpanParsableKey(int value) => Value = value;

               public static CustomSpanParsableKey Parse(string s, IFormatProvider? provider)
                  => new(int.Parse(s, provider));

               public static bool TryParse(string? s, IFormatProvider? provider, out CustomSpanParsableKey result)
               {
                  if (int.TryParse(s, provider, out var value))
                  {
                     result = new CustomSpanParsableKey(value);
                     return true;
                  }

                  result = default;
                  return false;
               }

               public static CustomSpanParsableKey Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
                  => new(int.Parse(s, provider));

               public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomSpanParsableKey result)
               {
                  if (int.TryParse(s, provider, out var value))
                  {
                     result = new CustomSpanParsableKey(value);
                     return true;
                  }

                  result = default;
                  return false;
               }

               public bool Equals(CustomSpanParsableKey other) => Value == other.Value;
               public override bool Equals(object? obj) => obj is CustomSpanParsableKey other && Equals(other);
               public override int GetHashCode() => Value.GetHashCode();
            }

            [SmartEnum<CustomSpanParsableKey>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_long_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<long>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_decimal_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<decimal>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_byte_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<byte>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_skip_IParsable_when_requested()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SkipIParsable = true)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_skip_IComparable_when_requested()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SkipIComparable = true)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_skip_IFormattable_when_requested()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SkipIFormattable = true)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_skip_ToString_when_requested()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SkipToString = true)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_ComparisonOperators_set_to_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_EqualityComparisonOperators_if_set_to_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_with_EqualityComparisonOperators_set_to_None_and_ComparisonOperators_is_set_to_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.None, ComparisonOperators = OperatorsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_equality_operators_with_DefaultWithKeyTypeOverloads()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(EqualityComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_key_as_property_when_KeyMemberKind_is_Property()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberKind = MemberKind.Property)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_public_key_member_when_KeyMemberAccessModifier_is_Public()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(KeyMemberAccessModifier = AccessModifier.Public)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_protected_key_member_when_KeyMemberAccessModifier_is_Protected()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberAccessModifier = AccessModifier.Protected)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_use_custom_KeyMemberName()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(KeyMemberName = "Identifier")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_serialization_when_SerializationFrameworks_is_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public void Should_not_generate_record_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial record TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_generate_with_file_scoped_namespace()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests;

         [SmartEnum<string>]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_nested_in_non_generic_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public partial class OuterClass
            {
               [SmartEnum<string>]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default!;
                  public static readonly TestEnum Item2 = default!;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.OuterClass.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyless_with_derived_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            public abstract partial class TestEnum
            {
               public static readonly TestEnum Item1 = new DerivedEnum1();
               public static readonly TestEnum Item2 = new DerivedEnum2();

               private sealed class DerivedEnum1 : TestEnum
               {
               }

               private sealed class DerivedEnum2 : TestEnum
               {
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_sealed_keyless_enum()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            public sealed partial class TestEnum
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_sealed_keyed_enum()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public sealed partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_KeyMemberEqualityComparer()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_KeyMemberComparer()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_both_KeyMemberEqualityComparer_and_KeyMemberComparer()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_multiple_ObjectFactory_attributes()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>]
            [ObjectFactory<long>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_DateTime_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<DateTime>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_deeply_nested_namespace()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests.Level1.Level2.Level3
         {
            [SmartEnum<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public void Should_handle_nested_in_generic_class_error()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public class OuterClass<T>
            {
               [SmartEnum<string>]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default!;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(
         source,
         [typeof(ISmartEnum<>).Assembly],
         ["Type 'TestEnum' must not be inside a generic type"]);
      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_not_generate_struct_with_string_key()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial struct TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, [typeof(ISmartEnum<>).Assembly], ["Attribute 'SmartEnum<>' is not valid on this declaration type. It is only valid on 'class' declarations."]);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_not_generate_keyless_struct()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            public partial struct TestEnum
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, [typeof(ISmartEnum<>).Assembly], expectedCompilerErrors: ["Attribute 'SmartEnum' is not valid on this declaration type. It is only valid on 'class' declarations."]);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_generate_short_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<short>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_ushort_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<ushort>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_uint_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<uint>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_ulong_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<ulong>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_float_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<float>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_double_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<double>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_DateTimeOffset_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<DateTimeOffset>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_TimeSpan_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<TimeSpan>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_KeyMemberKind_Field()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(KeyMemberKind = MemberKind.Field, KeyMemberAccessModifier = AccessModifier.Public)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_KeyMemberAccessModifier_Internal()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberAccessModifier = AccessModifier.Internal)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_SwitchMethods_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_SwitchMethods_Default()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.Default)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_MapMethods_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(MapMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_MapMethods_DefaultOnly()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(MapMethods = SwitchMapMethodsGeneration.Default)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_MapMethods_DefaultWithPartialOverloads()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_abstract_keyed_enum_with_derived_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public abstract partial class TestEnum
            {
               public static readonly TestEnum Item1 = new DerivedEnum1("Item1");
               public static readonly TestEnum Item2 = new DerivedEnum2("Item2");

               private sealed class DerivedEnum1 : TestEnum
               {
                  public DerivedEnum1(string key)
                  {
                  }
               }

               private sealed class DerivedEnum2 : TestEnum
               {
                  public DerivedEnum2(string key)
                  {
                  }
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyless_enum_with_single_item()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            public sealed partial class TestEnum
            {
               public static readonly TestEnum SingleItem = new();
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_enum_with_single_item()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public sealed partial class TestEnum
            {
               public static readonly TestEnum SingleItem = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_multiple_nested_smart_enums_in_same_outer_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public partial class OuterClass
            {
               [SmartEnum<string>]
               public partial class TestEnum1
               {
                  public static readonly TestEnum1 Item1 = default!;
                  public static readonly TestEnum1 Item2 = default!;
               }

               [SmartEnum<int>]
               public partial class TestEnum2
               {
                  public static readonly TestEnum2 Item1 = default!;
                  public static readonly TestEnum2 Item2 = default!;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.OuterClass.TestEnum1.SmartEnum.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum1.Comparable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum1.Parsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum1.SpanParsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum1.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.SmartEnum.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.Comparable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.Parsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.SpanParsable.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.OuterClass.TestEnum2.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_internal_smart_enum()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            internal partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_SerializationFrameworks_SystemTextJson()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_SerializationFrameworks_NewtonsoftJson()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SerializationFrameworks = SerializationFrameworks.NewtonsoftJson)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_SerializationFrameworks_MessagePack()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.MessagePack)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_ComparisonOperators_DefaultWithKeyTypeOverloads()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(ComparisonOperators = OperatorsGeneration.DefaultWithKeyTypeOverloads)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_deeply_nested_classes()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            public partial class Level1
            {
               public partial class Level2
               {
                  public partial class Level3
                  {
                     [SmartEnum<string>]
                     public partial class TestEnum
                     {
                        public static readonly TestEnum Item1 = default!;
                        public static readonly TestEnum Item2 = default!;
                     }
                  }
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.Level1.Level2.Level3.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_private_constructor()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;

               private TestEnum(string key)
               {
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_protected_constructor()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public abstract partial class TestEnum
            {
               public static readonly TestEnum Item1 = new DerivedEnum(1);

               protected TestEnum(int key)
               {
               }

               private sealed class DerivedEnum : TestEnum
               {
                  public DerivedEnum(int key) : base(key)
                  {
                  }
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_sbyte_based_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<sbyte>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_both_SwitchMethods_and_MapMethods_as_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.None, MapMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_mixed_SerializationFrameworks()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SerializationFrameworks = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_keyed_enum_with_many_items()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
               public static readonly TestEnum Item3 = default!;
               public static readonly TestEnum Item4 = default!;
               public static readonly TestEnum Item5 = default!;
               public static readonly TestEnum Item6 = default!;
               public static readonly TestEnum Item7 = default!;
               public static readonly TestEnum Item8 = default!;
               public static readonly TestEnum Item9 = default!;
               public static readonly TestEnum Item10 = default!;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_generic_keyless()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            public partial class TestKeylessEnum<T>
               where T : class
            {
               public static readonly TestKeylessEnum<T> Item1 = default!;
               public static readonly TestKeylessEnum<T> Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestKeylessEnum`1.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestKeylessEnum`1.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_generic_string_key_with_constraint()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class TestStringKeyEnum<T>
               where T : IComparable<T>
            {
               public static readonly TestStringKeyEnum<T> Item1 = default!;
               public static readonly TestStringKeyEnum<T> Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestStringKeyEnum`1.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestStringKeyEnum`1.Comparable.g.cs",
                        "Thinktecture.Tests.TestStringKeyEnum`1.Parsable.g.cs",
                        "Thinktecture.Tests.TestStringKeyEnum`1.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestStringKeyEnum`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestStringKeyEnum`1.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_generic_int_key_with_equatable_constraint()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial class TestIntKeyEnum<T>
               where T : IEquatable<T>
            {
               public static readonly TestIntKeyEnum<T> Item1 = default!;
               public static readonly TestIntKeyEnum<T> Item2 = default!;
               public static readonly TestIntKeyEnum<T> Item3 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestIntKeyEnum`1.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.Comparable.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.Formattable.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.Parsable.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestIntKeyEnum`1.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_IParsable_but_not_ISpanParsable_when_SkipISpanParsable_is_true()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>(SkipISpanParsable = true)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_IParsable_or_ISpanParsable_when_both_SkipIParsable_and_SkipISpanParsable_are_true()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>(SkipIParsable = true, SkipISpanParsable = true)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_IParsable_and_ISpanParsable_when_SkipIParsable_true_but_SkipISpanParsable_false()
   {
      // Dependency constraint: SkipISpanParsable should be forced to true, when SkipIParsable is true
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>(SkipIParsable = true, SkipISpanParsable = false)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_IParsable_only_when_SkipISpanParsable_true_for_Guid_key()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<Guid>(SkipISpanParsable = true)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_enum_with_string_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
            [ObjectFactory<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_enum_with_string_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>]
            [ObjectFactory<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_generate_for_string_based_enum_with_reaonlyspan_of_char_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_enum_with_string_and_reaonlyspan_of_char_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
            [ObjectFactory<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_enum_with_reaonlyspan_of_char_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_enum_with_string_and_reaonlyspan_of_char_based_object_factory()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<int>]
            [ObjectFactory<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs",
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }
#endif
}
