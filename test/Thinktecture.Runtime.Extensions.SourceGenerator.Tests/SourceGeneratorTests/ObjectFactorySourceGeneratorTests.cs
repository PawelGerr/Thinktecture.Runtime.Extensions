using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class ObjectFactorySourceGeneratorTests : SourceGeneratorTestsBase
{
   public ObjectFactorySourceGeneratorTests(ITestOutputHelper output)
      : base(output, 2_000)
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
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
               public static readonly TestEnum Item1 = new(1);
               public static readonly TestEnum Item2 = new(2);
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
}
