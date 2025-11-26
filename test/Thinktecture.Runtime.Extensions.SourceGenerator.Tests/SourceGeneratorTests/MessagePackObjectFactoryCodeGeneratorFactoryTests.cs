using System.Threading.Tasks;
using MessagePack;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.Formatters;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackObjectFactoryCodeGeneratorFactoryTests : SourceGeneratorTestsBase
{
   public MessagePackObjectFactoryCodeGeneratorFactoryTests(ITestOutputHelper output)
      : base(output, 2_000)
   {
   }

   [Fact]
   public void Instance_should_not_be_null()
   {
      MessagePackObjectFactoryCodeGeneratorFactory.Instance.Should().NotBeNull();
   }

   [Fact]
   public void Instance_should_return_same_instance()
   {
      var instance1 = MessagePackObjectFactoryCodeGeneratorFactory.Instance;
      var instance2 = MessagePackObjectFactoryCodeGeneratorFactory.Instance;

      instance1.Should().BeSameAs(instance2);
   }

   [Fact]
   public void CodeGeneratorName_should_return_correct_value()
   {
      var instance = MessagePackObjectFactoryCodeGeneratorFactory.Instance;

      instance.CodeGeneratorName.Should().Be("MessagePack-ObjectFactory-CodeGenerator");
   }

   [Fact]
   public void Instance_should_be_IKeyedSerializerCodeGeneratorFactory()
   {
      MessagePackObjectFactoryCodeGeneratorFactory.Instance.Should().BeAssignableTo<IKeyedSerializerCodeGeneratorFactory>();
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_smart_enum_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_value_object_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_ObjectFactory_with_SerializationFrameworks_All()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_MessagePack_formatter_for_ObjectFactory_with_SerializationFrameworks_None()
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
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_MessagePack_formatter_for_ObjectFactory_with_SerializationFrameworks_SystemTextJson_only()
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
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_MessagePackFormatterAttribute_is_present()
   {
      var source = """

         using System;
         using MessagePack;
         using MessagePack.Formatters;

         namespace Thinktecture.Tests
         {
            public class TestEnumMessagePackFormatter : IMessagePackFormatter<TestEnum>
            {
               public void Serialize(ref MessagePackWriter writer, TestEnum value, MessagePackSerializerOptions options)
               {
                  throw new NotImplementedException();
               }

               public TestEnum Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
               {
                  throw new NotImplementedException();
               }
            }

            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [MessagePackFormatter(typeof(TestEnumMessagePackFormatter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_multiple_ObjectFactories_with_MessagePack_enabled()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_all_ObjectFactories_exclude_MessagePack()
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
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_struct_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial struct TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_complex_value_object_with_ObjectFactory()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(ComplexValueObjectAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePack_formatter_for_union_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union<string, int>]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.MessagePack)]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute<,>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
