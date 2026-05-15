using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.AdHocUnions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class AdHocUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public AdHocUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 37_000)
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
   public async Task Should_generate_class_with_string_and_int_for_AdHocUnionAttribute()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(string), typeof(int))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_named_ctor_args_for_AdHocUnionAttribute()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(t2: typeof(int), t1: typeof(string))]
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

   [Fact]
   public async Task Should_skip_equals_method_with_SkipEqualityComparison()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, bool, int?>(SkipEqualityComparison = true)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_skip_equals_method_with_SkipEqualityComparison_using_non_generic_AdHocUnionAttribute()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(int), typeof(string), typeof(List<int>), typeof(bool), typeof(int?), SkipEqualityComparison = true)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_3_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, bool>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_4_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, bool, Guid>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public void Should_not_generate_record_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
            public partial record TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public void Should_not_generate_record_struct()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
            public partial record struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_generate_readonly_struct_with_string_and_int()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
            public readonly partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_three_same_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, string, string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_two_pairs_of_duplicates()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, string, int, int>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_complex_duplicate_scenario()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, string, bool, string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_duplicate_types_and_custom_name_for_first()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, string>(T1Name = "First")]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_all_nullable_reference_types()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<string, List<int>, object>(
               T1IsNullableReferenceType = true,
               T2IsNullableReferenceType = true,
               T3IsNullableReferenceType = true)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_nested_generic_types()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<Dictionary<string, List<int>>, int>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_tuple_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<(int, string), bool>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_delegate_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<Func<int, string>, Action<int>>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_jagged_array()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int[][], string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_multidimensional_array()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int[,], string>]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_all_5_custom_names()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, bool, Guid, char>(
               T1Name = "Text",
               T2Name = "Number",
               T3Name = "Flag",
               T4Name = "Id",
               T5Name = "Character")]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_AdHocUnion_and_3_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(string), typeof(int), typeof(bool))]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_AdHocUnion_and_4_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(string), typeof(int), typeof(bool), typeof(Guid))]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_mixed_nullable_value_and_reference_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int?, string, bool?>(T2IsNullableReferenceType = true)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_multiple_settings_combined()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(
               SkipToString = true,
               SwitchMethods = SwitchMapMethodsGeneration.None,
               MapMethods = SwitchMapMethodsGeneration.None,
               ConversionFromValue = ConversionOperatorsGeneration.None,
               ConversionToValue = ConversionOperatorsGeneration.None)]
            public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_T1_as_stateless_type()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, string>(T1IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_T2_as_stateless_type()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct EmptyState { }

         	[Union<string, EmptyState>(T2IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_multiple_stateless_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }
         	public readonly struct EmptyState { }

         	[Union<NullValue, EmptyState, string>(T1IsStateless = true, T2IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_stateless_type_using_AdHocUnionAttribute()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[AdHocUnion(typeof(NullValue), typeof(string), T1IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_union_with_stateless_type()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, int>(T1IsStateless = true)]
         	public partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_all_three_types_as_stateless()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }
         	public readonly struct EmptyState { }
         	public readonly struct UndefinedValue { }

         	[Union<NullValue, EmptyState, UndefinedValue>(T1IsStateless = true, T2IsStateless = true, T3IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_stateless_type_and_conversion_operators_disabled()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, string>(T1IsStateless = true, ConversionFromValue = ConversionOperatorsGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_stateless_type_and_explicit_conversion_operators()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, string>(T1IsStateless = true, ConversionFromValue = ConversionOperatorsGeneration.Explicit)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_duplicate_value_struct_stateless_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, NullValue, string>(T1IsStateless = true, T2IsStateless = true, T1Name = "NullValue1", T2Name = "NullValue2")]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_duplicate_stateless_types_T2_and_T3()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct EmptyState { }

         	[Union<string, EmptyState, EmptyState>(T2IsStateless = true, T3IsStateless = true, T2Name = "EmptyState1", T3Name = "EmptyState2")]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_duplicate_reference_type_stateless_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public class NullValueClass { }

         	[Union<NullValueClass, NullValueClass, int>(T1IsStateless = true, T2IsStateless = true, T1Name = "NullValueClass1", T2Name = "NullValueClass2")]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_union_with_duplicate_stateless_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public readonly struct NullValue { }

         	[Union<NullValue, NullValue, int>(T1IsStateless = true, T2IsStateless = true, T1Name = "NullValue1", T2Name = "NullValue2")]
         	public partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_duplicate_regular_string_types()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int, string, string, int?>(T1Name = "Text", T4IsNullableReferenceType = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_duplicate_regular_types_requiring_factory_methods()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, int, string>(T1Name = "Value1", T3Name = "Value2")]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_union_with_nullable_and_non_nullable_of_same_type()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int, int?, string>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_TypeParamRef1_and_TypeParamRef2()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, TypeParamRef2>]
         	public partial class TestUnion<T1, T2>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`2.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_nested_TypeParamRef1()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, List<TypeParamRef1>>]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_constrained_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : struct;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public void Should_not_generate_when_TypeParamRef_index_out_of_range()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef3, string>]
         	public partial struct TestUnion<T>;
         }
         """;
      var result = GetGeneratedOutputsWithDiagnostics<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      result.Outputs.Should().BeEmpty();

      result.GeneratorDiagnostics.Should().ContainSingle(d => d.Id == "TTRESG071")
            .Which.GetMessage().Should().Be("TypeParamRef3 references type parameter at index 3, but the type 'TestUnion<T>' has only 1 type parameter(s)");
   }

   [Fact]
   public void Should_not_generate_when_TypeParamRef_used_on_non_generic_type()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion;
         }
         """;
      var result = GetGeneratedOutputsWithDiagnostics<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      result.Outputs.Should().BeEmpty();

      result.GeneratorDiagnostics.Should().ContainSingle(d => d.Id == "TTRESG072")
            .Which.GetMessage().Should().Be("TypeParamRef1 cannot be used on non-generic type 'TestUnion'");
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_not_generate_when_type_parameter_allows_ref_struct()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : allows ref struct;
         }
         """;
      var result = GetGeneratedOutputsWithDiagnostics<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      result.Outputs.Should().BeEmpty();

      result.GeneratorDiagnostics.Should().ContainSingle(d => d.Id == "TTRESG073")
            .Which.GetMessage().Should().Be("Ad-hoc union 'TestUnion<T>' has type parameter 'T' with 'allows ref struct' which is not supported. Remove the 'allows ref struct' anti-constraint from the type parameter.");
   }
#endif

   [Fact]
   public async Task Should_generate_with_warning_when_generic_type_does_not_use_TypeParamRef()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>]
         	public partial struct TestUnion<T>;
         }
         """;
      var result = GetGeneratedOutputsWithDiagnostics<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      // TTRESG107 is a warning, not an error - code is still generated
      await VerifyAsync(result.Outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");

      result.GeneratorDiagnostics.Should().ContainSingle(d => d.Id == "TTRESG107")
            .Which.GetMessage().Should().Be("Generic ad-hoc union 'TestUnion<T>' does not reference any type parameter via TypeParamRef. All type parameters are unused.");
   }

   [Fact]
   public async Task Should_generate_struct_with_nullable_nested_TypeParamRef1()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(List<TypeParamRef1?>), typeof(string))]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_array_of_TypeParamRef1()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1[], string>]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_tuple_TypeParamRef()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<(TypeParamRef1, string), int>]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_dictionary_of_TypeParamRefs()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<Dictionary<TypeParamRef1, TypeParamRef2>, string>]
         	public partial struct TestUnion<T1, T2>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`2.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_nullable_nested_TypeParamRef1_and_class_constraint()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(List<TypeParamRef1?>), typeof(string))]
         	public partial struct TestUnion<T> where T : class;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_nullable_nested_TypeParamRef1_and_struct_constraint()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(List<TypeParamRef1?>), typeof(string))]
         	public partial struct TestUnion<T> where T : struct;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_duplicate_TypeParamRef1()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, TypeParamRef1>]
         	public partial struct TestUnion<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_notnull_constrained_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : notnull;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_nullable_nested_TypeParamRef1_and_notnull_constraint()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[AdHocUnion(typeof(List<TypeParamRef1?>), typeof(string))]
         	public partial struct TestUnion<T> where T : notnull;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_class_nullable_constrained_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : class?;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_new_constrained_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : new();
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_struct_with_interface_constrained_TypeParamRef1_and_string()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<TypeParamRef1, string>]
         	public partial struct TestUnion<T> where T : IComparable<T>;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_FactoryMethodGeneration_Always()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(FactoryMethodGeneration = FactoryMethodGeneration.Always)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_FactoryMethodGeneration_None()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<string, int>(FactoryMethodGeneration = FactoryMethodGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_emit_Normalize_for_duplicate_member_with_FactoryMethodGeneration_None()
   {
      // With FactoryMethodGeneration = None and duplicate types, the duplicate members have no
      // accessible call site (private indexed ctor, no factory). No NormalizeXxx must be emitted
      // for those members. Counter == 0 members still get Normalize in the public ctor.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, string>(FactoryMethodGeneration = FactoryMethodGeneration.None)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_emit_Normalize_in_ctor_when_factory_generation_triggered_by_interface_member()
   {
      // An interface member triggers factory generation for all members. Both members have counter == 0,
      // so both get their Normalize call in the public ctor; the generated factories simply delegate to
      // the ctor and must not double-fire.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }

         	[Union<string, IFoo>]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_typed_backing_field_when_SingleBackingFieldType_is_set()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_typed_backing_field_with_abstract_base_class()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public abstract class FooBase { public abstract string Bar { get; } }
         	public class Foo1 : FooBase { public override string Bar => "foo1"; }
         	public class Foo2 : FooBase { public override string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(SingleBackingFieldType = typeof(FooBase))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_typed_backing_field_with_value_type_members()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int, double>(SingleBackingFieldType = typeof(IComparable))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_emit_cached_boxed_default_for_stateless_struct_with_SingleBackingFieldType()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public readonly struct EmptyState : IFoo { public string Bar => "empty"; }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }

         	[Union<EmptyState, Foo1>(
         	   SingleBackingFieldType = typeof(IFoo),
         	   T1IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_leave_obj_unassigned_for_stateless_reference_type_with_SingleBackingFieldType()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class NullFoo : IFoo { public string Bar => "null"; }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }

         	[Union<NullFoo, Foo1>(
         	   SingleBackingFieldType = typeof(IFoo),
         	   T1IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_make_Value_nullable_when_TXIsNullableReferenceType_set_with_SingleBackingFieldType()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(
         	   SingleBackingFieldType = typeof(IFoo),
         	   T1IsNullableReferenceType = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public void Should_normalize_typeof_object_in_SingleBackingFieldType_to_default_behavior()
   {
      // The design specifies: setting SingleBackingFieldType implies UseSingleBackingField = true,
      // and typeof(object) is normalized so it behaves identically to UseSingleBackingField = true alone.
      // Explicit-pair: caller sets both SingleBackingFieldType = typeof(object) AND UseSingleBackingField = true
      // (mirrors what the cascade is supposed to do). Output must match UseSingleBackingField = true alone.
      var sourceWithTypeofObject = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, bool, int?>(SingleBackingFieldType = typeof(object), UseSingleBackingField = true)]
            public partial class TestUnion;
         }
         """;
      var sourceWithUseSingleBackingField = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<int, string, List<int>, bool, int?>(UseSingleBackingField = true)]
            public partial class TestUnion;
         }
         """;
      var outputsObject = GetGeneratedOutputs<AdHocUnionSourceGenerator>(sourceWithTypeofObject, typeof(UnionAttribute<,>).Assembly);
      var outputsFlag = GetGeneratedOutputs<AdHocUnionSourceGenerator>(sourceWithUseSingleBackingField, typeof(UnionAttribute<,>).Assembly);

      outputsObject.Should().HaveCount(outputsFlag.Count);

      foreach (var (key, value) in outputsObject)
      {
         outputsFlag.Should().ContainKey(key);
         outputsFlag[key].Should().Be(value);
      }
   }

   [Fact]
   public async Task Should_apply_cascade_when_SingleBackingFieldType_set_without_UseSingleBackingField()
   {
      // No explicit UseSingleBackingField — cascade should kick in.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_resolve_TypeParamRef_in_SingleBackingFieldType()
   {
      var source = """
         using System;
         using System.Collections.Generic;

         namespace Thinktecture.Tests
         {
         	[Union<List<TypeParamRef1>, TypeParamRef1[]>(SingleBackingFieldType = typeof(IEnumerable<TypeParamRef1>))]
         	public partial class TestUnion<T> where T : notnull;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion`1.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_all_stateless_struct_members_with_SingleBackingFieldType()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public readonly struct A : IFoo { public string Bar => "a"; }
         	public readonly struct B : IFoo { public string Bar => "b"; }

         	[Union<A, B>(
         	   SingleBackingFieldType = typeof(IFoo),
         	   T1IsStateless = true,
         	   T2IsStateless = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_mixed_ref_and_value_types_with_reference_base_in_SingleBackingFieldType()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public readonly struct Foo2Struct : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2Struct>(SingleBackingFieldType = typeof(IFoo))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_nullable_struct_backing_field_when_SingleBackingFieldType_is_nullable_struct()
   {
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	[Union<int, short>(SingleBackingFieldType = typeof(int?))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_keep_value_type_backing_field_as_is_with_nullable_members()
   {
      // SingleBackingFieldType = typeof(int) (non-nullable struct) combined with a nullable
      // reference member (string?) should NOT auto-upgrade _obj to int? -- the user's chosen
      // non-nullable type wins. (Member-type assignability is left to the compiler.)
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public class Foo {}

         	[Union<int, Foo>(
         	   SingleBackingFieldType = typeof(int),
         	   T2IsNullableReferenceType = true)]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_emit_Value_index_check_with_throw_for_struct_union_with_SingleBackingFieldType()
   {
      // Struct unions with SingleBackingFieldType: the Value getter must NOT short-circuit to
      // `this._obj` -- it must check `_valueIndex == 0` and throw InvalidOperationException so a
      // `default(StructUnion).Value` matches the contract documented on the property and the
      // behavior of AsTx/Switch/Map/ToString/Equals.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo))]
         	public readonly partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_emit_Value_index_check_with_throw_for_struct_union_with_stateless_struct_member_and_SingleBackingFieldType()
   {
      // Struct union with a stateless struct member: the Value getter returns `this._obj`
      // uniformly (the ctor for the stateless member assigns _obj = _cachedBoxedX). The
      // `_valueIndex == 0` check must still throw to match the documented contract on
      // default(StructUnion).
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public readonly struct EmptyState : IFoo { public string Bar => "empty"; }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }

         	[Union<EmptyState, Foo1>(
         	   SingleBackingFieldType = typeof(IFoo),
         	   T1IsStateless = true)]
         	public readonly partial struct TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_keep_Value_short_circuit_for_class_union_with_SingleBackingFieldType()
   {
      // Class unions with SingleBackingFieldType: the Value getter keeps the short-circuit
      // `Value => this._obj;`. Class instances cannot be uninitialized via `default(...)`, so no
      // discriminator throw is needed. This snapshot guards against accidental regressions if the
      // struct fix is generalized to all unions.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_typed_backing_field_for_AdHocUnionAttribute_with_SingleBackingFieldType()
   {
      // Non-generic [AdHocUnion(...)] form: SingleBackingFieldType lives on UnionAttributeBase
      // and must work identically to the generic [Union<...>] form. Snapshot guards parity.
      var source = """
         using System;

         namespace Thinktecture.Tests
         {
         	public interface IFoo { string Bar { get; } }
         	public class Foo1 : IFoo { public string Bar => "foo1"; }
         	public class Foo2 : IFoo { public string Bar => "foo2"; }

         	[AdHocUnion(typeof(Foo1), typeof(Foo2), SingleBackingFieldType = typeof(IFoo))]
         	public partial class TestUnion;
         }
         """;
      var outputs = GetGeneratedOutputs<AdHocUnionSourceGenerator>(source, typeof(UnionAttribute<,>).Assembly);

      await VerifyAsync(outputs, "Thinktecture.Tests.TestUnion.AdHocUnion.g.cs");
   }
}
