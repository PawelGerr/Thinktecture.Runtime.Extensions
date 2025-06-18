using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class SmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public SmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 43_000)
   {
   }

   [Fact]
   public void Should_not_generate_if_generic()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[SmartEnum<string>]
           public partial class TestEnum<T>
         	{
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
            }
         }
         """;

      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);
      outputs.Should().BeEmpty();
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
   public void Should_not_crash_if_type_flagged_with_multiple_source_gen_attributes()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            [SmartEnum<string>]
            [ValueObject]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
   public async Task Should_generate_string_based_class_without_namespace()
   {
      var source = """
         using System;
         using Thinktecture;

         [SmartEnum<string>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                            MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "TestEnum.SmartEnum.g.cs",
                        "TestEnum.Comparable.g.cs",
                        "TestEnum.Parsable.g.cs",
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
               public static readonly TestEnum Item_1 = new("Item 1");
               public static readonly TestEnum Item_2 = new("Item 2");
               public static readonly TestEnum Item_int_1 = new GenericEnum<int>("GenericEnum<int> 1");
               public static readonly TestEnum Item_decimal_1 = new GenericEnum<decimal>("GenericEnum<decimal> 1");
               public static readonly TestEnum Item_decimal_2 = new GenericEnum<decimal>("GenericEnum<decimal> 2");
               public static readonly TestEnum Item_derived_1 = new DerivedEnum("DerivedEnum 1");
               public static readonly TestEnum Item_derived_2 = new DerivedEnum("DerivedEnum 2");

               private class GenericEnum<T> : TestEnum
               {
                  public DerivedEnum(string key)
                     : base(key)
                  {
                  }
               }

               private class UnusedGenericEnum<T> : TestEnum
               {
                  public UnusedGenericEnum(string key)
                     : base(key)
                  {
                  }
               }

               private class DerivedEnum : TestEnum
               {
                  public DerivedEnum(string key)
                     : base(key)
                  {
                  }
               }

               private class UnusedDerivedEnum : TestEnum
               {
                  public UnusedDerivedEnum(string key)
                     : base(key)
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source, typeof(ISmartEnum<>).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.SmartEnum.g.cs",
                        "Thinktecture.Tests.TestEnum.Comparable.g.cs",
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
               public static readonly TestEnum Item1 = null!;
               public static readonly TestEnum Item2 = null!;

               public abstract int Value { get; }

               private sealed class ConcreteEnum : TestEnum
               {
                  public override int Value => 100;

                  public ConcreteEnum(int key)
                     : base(key)
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
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_take_over_nullability_of_generic_members()
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

               public Func<string?, Task<string?>?>? Prop1 { get; }
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
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs");
   }

   [Fact]
   public async Task Should_generate_delegate_of_type_func()
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
               public partial Task<string?>? Method1(string? arg1);
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
                        "Thinktecture.Tests.TestEnum.ComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.EqualityComparisonOperators.g.cs",
                        "Thinktecture.Tests.TestEnum.Formattable.g.cs");
   }
}
