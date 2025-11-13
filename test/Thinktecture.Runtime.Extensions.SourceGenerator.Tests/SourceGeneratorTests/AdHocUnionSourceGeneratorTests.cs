using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class AdHocUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public AdHocUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 28_000)
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
}
