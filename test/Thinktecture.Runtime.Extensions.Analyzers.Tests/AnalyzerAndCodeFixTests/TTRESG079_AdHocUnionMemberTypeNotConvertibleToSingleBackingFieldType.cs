using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG079_AdHocUnionMemberTypeNotConvertibleToSingleBackingFieldType
{
   private const string _DIAGNOSTIC_ID = "TTRESG079";

   [Fact]
   public async Task Should_trigger_for_every_member_that_cannot_be_stored_in_the_backing_field()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public struct EmptyState;

            [Union<EmptyState, string>(T1IsStateless = true, UseSingleBackingField = true, {|#0:SingleBackingFieldType = typeof(int)|})]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly],
         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("EmptyState", "TestUnion", "int"),
         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("string", "TestUnion", "int"));
   }

   [Fact]
   public async Task Should_trigger_when_member_type_is_unrelated_to_the_backing_field_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, string>({|#0:SingleBackingFieldType = typeof(string)|})]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("int", "TestUnion", "string");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_conversion_to_backing_field_type_is_user_defined()
   {
      // A user-defined conversion would store a different instance, which breaks 'Value', 'AsTx' and equality.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public readonly struct Celsius
            {
               public static implicit operator int(Celsius value) => 0;
            }

            [Union<Celsius, int>({|#0:SingleBackingFieldType = typeof(int)|})]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Celsius", "TestUnion", "int");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_stateless_reference_type_member_when_all_members_are_stateless()
   {
      // Without a non-stateless member there is no shared backing field, so the raw value getter
      // has to convert 'default(T)' of every member to the backing field type.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class NotFound;
            public class Invalid;

            [Union<NotFound, Invalid>(T1IsStateless = true, T2IsStateless = true, {|#0:SingleBackingFieldType = typeof(string)|})]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly],
                                         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("NotFound", "TestUnion", "string"),
                                         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Invalid", "TestUnion", "string"));
   }

   [Fact]
   public async Task Should_not_trigger_for_stateless_reference_type_member_when_union_has_a_non_stateless_member()
   {
      // The value of a stateless reference-type member never reaches the backing field.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class NotFound;
            public class Product;

            [Union<NotFound, Product>(T1IsStateless = true, SingleBackingFieldType = typeof(Product))]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_backing_field_type_is_an_implemented_interface()
   {
      // Boxing conversion for the struct member, reference conversion for the class member.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface IFoo;
            public struct FooStruct : IFoo;
            public class FooClass : IFoo;

            [Union<FooStruct, FooClass>(SingleBackingFieldType = typeof(IFoo))]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_implicit_nullable_and_numeric_conversions()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, short>(SingleBackingFieldType = typeof(int?))]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_backing_field_type_is_object()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, string>(SingleBackingFieldType = typeof(object))]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_member_type_containing_a_TypeParamRef_marker()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<TypeParamRef1, int>(SingleBackingFieldType = typeof(int))]
            public partial class TestUnion<T>
               where T : notnull;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_backing_field_type_contains_a_TypeParamRef_marker()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<TypeParamRef1, string>(SingleBackingFieldType = typeof(TypeParamRef1))]
            public partial class TestUnion<T>
               where T : notnull;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_UseSingleBackingField_is_false()
   {
      // TTRESG075 owns this configuration; reporting TTRESG079 on top would be noise.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [{|#0:Union<int, string>(SingleBackingFieldType = typeof(string), UseSingleBackingField = false)|}]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic("TTRESG075").WithLocation(0).WithArguments("TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }
}
