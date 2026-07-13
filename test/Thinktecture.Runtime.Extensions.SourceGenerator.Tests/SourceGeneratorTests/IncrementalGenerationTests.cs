using System.Linq;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

// These tests reuse a single GeneratorDriver across two runs (initial source, then an edited source)
// so that the incremental cache is exercised. They guard against "state equality staleness": a state
// object whose Equals/GetHashCode omits a relevant member makes the driver keep stale output after an edit.
public class IncrementalGenerationTests : SourceGeneratorTestsBase
{
   public IncrementalGenerationTests(ITestOutputHelper output)
      : base(output, 21_000)
   {
   }

   [Fact]
   public void Should_regenerate_auxiliary_files_when_containing_type_kind_changes()
   {
      var initialSource = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public partial class Container
            {
               [ValueObject<int>]
               public partial struct TestValueObject
               {
               }
            }
         }

         """;

      var editedSource = initialSource.Replace("public partial class Container", "public partial record Container");

      var outputs = GetGeneratedOutputsAfterEdit<ValueObjectSourceGenerator>(
         initialSource,
         editedSource,
         ".Formattable",
         typeof(ValueObjectAttribute<int>).Assembly);

      var formattable = outputs.Values.Single();

      formattable.Should().Contain("partial record Container");
      formattable.Should().NotContain("partial class Container");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_stop_generating_ISpanParsable_when_SkipISpanParsable_is_enabled()
   {
      var initialSource = """

         using System;
         using Thinktecture;

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

      var editedSource = initialSource.Replace("[SmartEnum<string>]", "[SmartEnum<string>(SkipISpanParsable = true)]");

      var outputs = GetGeneratedOutputsAfterEdit<ObjectFactorySourceGenerator>(
         initialSource,
         editedSource,
         ".SpanParsable",
         typeof(ObjectFactoryAttribute).Assembly);

      outputs.Should().BeEmpty();
   }
#endif
}
