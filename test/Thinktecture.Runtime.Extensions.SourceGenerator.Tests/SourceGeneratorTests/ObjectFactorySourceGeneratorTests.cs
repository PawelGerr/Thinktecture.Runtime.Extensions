using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class ObjectFactorySourceGeneratorTests : SourceGeneratorTestsBase
{
   public ObjectFactorySourceGeneratorTests(ITestOutputHelper output)
      : base(output, 2_500)
   {
   }

   [Fact]
   public async Task Should_generate_object_factory_for_smart_enum_class_based_on_int()
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_ad_hoc_union()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union<string, int>]
            [ObjectFactory<string>]
            public partial class TestUnion;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestUnion.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_regular_union()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union]
            [ObjectFactory<string>]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.ObjectFactories.g.cs",
                        "Thinktecture.Tests.Result`1.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_constructor_call_when_HasCorrespondingConstructor_is_true()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ObjectFactory<string>(HasCorrespondingConstructor = true)]
         	public partial class TestClass;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestClass.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestClass.Parsable.g.cs");
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_struct()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
         	public partial struct TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_complex_struct()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            [ObjectFactory<string>]
         	public partial struct TestValueObject
         	{
               public int Value1 { get; }
               public string Value2 { get; }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_record_class()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
         	public partial record TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_record_struct()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
         	public partial record struct TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_multiple_object_factories_with_different_types()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>]
            [ObjectFactory<int>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_constructor_call_when_HasCorrespondingConstructor_is_false()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ObjectFactory<string>(HasCorrespondingConstructor = false)]
         	public partial class TestClass;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestClass.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestClass.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_UseForSerialization_SystemTextJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_UseForSerialization_None()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_SkipIParsable()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(SkipIParsable = true)]
            [ObjectFactory<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_keyed_value_object_with_string()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [ObjectFactory<int>]
         	public partial class TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs");
   }

   [Fact]
   public async Task Should_generate_code_for_generic_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
         	public partial class TestValueObject<T>;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(
         source,
         [typeof(ObjectFactoryAttribute).Assembly],
         ["Error during code generation for 'TestValueObject': 'Keyed value objects must not be generic'"]);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject`1.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject`1.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_complex_generic_type()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            [ObjectFactory<string>]
         	public partial class TestValueObject<T>
         	{
               public T Value { get; }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject`1.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject`1.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_ValidationError()
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
               public int Value { get; }

               static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref int value)
               {
                  if (value < 0)
                     validationError = new ValidationError("Value must be positive");
               }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(
         source,
         [typeof(ObjectFactoryAttribute).Assembly],
         ["No defining declaration found for implementing declaration of partial method 'TestValueObject.ValidateFactoryArguments(ref ValidationError?, ref int)'"]);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_empty_class()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ObjectFactory<string>]
         	public partial class TestClass;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestClass.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestClass.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_nested_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            public partial class OuterClass
            {
               [ValueObject<int>]
               [ObjectFactory<string>]
               public partial class TestValueObject;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.OuterClass.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.OuterClass.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_internal_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
         	internal partial class TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_Guid_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<Guid>]
         	public partial class TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_union_with_custom_factory_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union<string, int>]
            [ObjectFactory<double>]
            public partial class TestUnion;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.ObjectFactories.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_byte_array_type()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [ObjectFactory<byte[]>]
         	public partial class TestValueObject;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs");
   }

   [Fact]
   public async Task Should_not_crash_if_type_has_errors()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ObjectFactory<string>]
         	public partial class TestClass : NonExistentType;
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(
         source,
         [typeof(ObjectFactoryAttribute).Assembly],
         ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestClass.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestClass.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_for_positional_record()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            [ObjectFactory<string>]
         	public partial record TestValueObject(int Value1, string Value2);
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_object_factory_with_multiple_factories_and_different_serialization_settings()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_value_object_with_string_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [ObjectFactory<string>]
            public partial class TestValueObject
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject? item)
               {
                  throw new NotImplementedException();
               }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_value_object_with_string_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>]
            public partial class TestValueObject
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject? item)
               {
                  throw new NotImplementedException();
               }
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_generate_for_string_based_enum_with_readonlyspan_of_char_based_object_factory()
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_enum_with_string_and_readonlyspan_of_char_based_object_factory()
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_enum_with_readonlyspan_of_char_based_object_factory()
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_enum_with_string_and_readonlyspan_of_char_based_object_factory()
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
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestEnum.Parsable.g.cs",
                        "Thinktecture.Tests.TestEnum.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_value_object_with_readonlyspan_of_char_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
         	[ValueObject<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestValueObject
            {
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_string_based_value_object_with_string_and_readonlyspan_of_char_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
         	[ValueObject<string>]
            [ObjectFactory<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestValueObject
            {
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_value_object_with_readonlyspan_of_char_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
         	[ValueObject<int>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestValueObject
            {
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.SpanParsable.g.cs");
   }

   [Fact]
   public async Task Should_generate_for_int_based_value_object_with_string_and_readonlyspan_of_char_based_object_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
         	[ValueObject<int>]
            [ObjectFactory<string>]
            [ObjectFactory<ReadOnlySpan<char>>]
            public partial class TestValueObject
            {
            }
         }

         """;
      var outputs = GetGeneratedOutputs<ObjectFactorySourceGenerator>(source, typeof(ObjectFactoryAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestValueObject.ObjectFactories.g.cs",
                        "Thinktecture.Tests.TestValueObject.Parsable.g.cs",
                        "Thinktecture.Tests.TestValueObject.SpanParsable.g.cs");
   }

#endif
}
