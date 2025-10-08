using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class AdHocUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public AdHocUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 27_000)
   {
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_string_and_int()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
         	public partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_ref_struct_with_string_and_int()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
         	public ref partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_without_implicit_conversion_from_value()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(ConversionFromValue = ConversionOperatorsGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_explicit_conversion_from_value()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(ConversionFromValue = ConversionOperatorsGeneration.Explicit)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_without_explicit_conversion_to_value()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(ConversionToValue = ConversionOperatorsGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_implilcit_conversion_to_value()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(ConversionToValue = ConversionOperatorsGeneration.Implicit)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_private_ctors()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(ConstructorAccessModifier = UnionConstructorAccessModifier.Private)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_SwitchPartially()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_MapPartially()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_without_Map()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(MapMethods = SwitchMapMethodsGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_without_Switch()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_without_ToString()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(SkipToString = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_and_custom_string_comparison()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(DefaultStringComparison = StringComparison.Ordinal)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_nullable_string_and_nullable_int()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int?>(T1IsNullableReferenceType = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_with_custom_names()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(T1Name = "Text", T2Name = "Number")]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_string_and_int_bool_guid_char()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, bool, Guid, char>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_array()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string[], int>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_same_member_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, string, string, int?>(
         	   T1Name = "Text",
         	   T4IsNullableReferenceType = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_generics()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<List<string>, List<int>>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_special_chars()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
            public class _1Test;

         	[Union<int, _1Test>]
            public partial class _1TestUnionWithSpecialChars;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests._1TestUnionWithSpecialChars.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_custom_SwitchMapStateParameterName()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string>(SwitchMapStateParameterName = "context")]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_object_for_2_different_reference_types_but_not_structs()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_nullable_struct()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int?, string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_object_for_2_out_of_3_different_reference_types()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_use_object_for_2_same_reference_types()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_use_object_for_structs_by_default()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, bool, int?>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_object_even_for_structs_with_UseSingleBackingField()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, bool, int?>(UseSingleBackingField = true)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }
}
