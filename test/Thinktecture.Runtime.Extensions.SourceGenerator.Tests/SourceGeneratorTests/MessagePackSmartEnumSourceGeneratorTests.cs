using MessagePack;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.Formatters;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackSmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public MessagePackSmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_Attribute_if_Attribute_is_missing()
   {
      var source = @"
using System;

namespace Thinktecture.Tests
{
   [SmartEnum<string>]
	public partial class TestEnum
	{
      public static readonly TestEnum Item1 = new(""Item1"");
      public static readonly TestEnum Item2 = new(""Item2"");
   }
}
";
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".MessagePack",
                                                                typeof(IEnum<>).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                           partial class TestEnum
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_for_enum_without_namespace()
   {
      var source = @"
using System;
using Thinktecture;

[SmartEnum<string>]
public partial class TestEnum
{
   public static readonly TestEnum Item1 = new(""Item1"");
   public static readonly TestEnum Item2 = new(""Item2"");
}
";
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".MessagePack",
                                                                typeof(IEnum<>).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<global::TestEnum, string, global::Thinktecture.ValidationError>))]
                           partial class TestEnum
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_Attribute_for_struct_if_Attribute_is_missing()
   {
      var source = @"
using System;

namespace Thinktecture.Tests
{
   [SmartEnum<string>]
	public readonly partial struct TestEnum
	{
      public static readonly TestEnum Item1 = new(""Item1"");
      public static readonly TestEnum Item2 = new(""Item2"");
   }
}
";
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".MessagePack",
                                                                typeof(IEnum<>).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.StructValueObjectMessagePackFormatter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
                           partial struct TestEnum
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_not_generate_MessagePackFormatter_and_attribute_if_Attribute_is_present()
   {
      var source = @"
using System;
using MessagePack;

namespace Thinktecture.Tests
{
   public class TestEnumMessagePackFormatter : Thinktecture.Formatters.EnumMessagePackFormatter<TestEnum, string>
   {
      public TestEnumMessagePackFormatter()
         : base(TestEnum.Get)
      {
      }
   }

   [SmartEnum<string>]
   [MessagePackFormatter(typeof(TestEnumMessagePackFormatter))]
	public partial class TestEnum
	{
      public static readonly TestEnum Item1 = new(""Item1"");
      public static readonly TestEnum Item2 = new(""Item2"");
   }
}
";
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".MessagePack",
                                                                typeof(IEnum<>).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_MessagePackFormatter_for_keyless_enum()
   {
      var source = @"
using System;

namespace Thinktecture.Tests
{
   [SmartEnum]
	public partial class TestEnum
	{
      public static readonly TestEnum Item1 = new(""Item1"");
      public static readonly TestEnum Item2 = new(""Item2"");
   }
}
";
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".MessagePack",
                                                                typeof(IEnum<>).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }
}
