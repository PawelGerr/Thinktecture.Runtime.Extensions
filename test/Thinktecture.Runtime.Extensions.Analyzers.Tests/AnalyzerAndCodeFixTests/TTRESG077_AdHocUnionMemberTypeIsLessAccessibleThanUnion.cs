using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG077_AdHocUnionMemberTypeIsLessAccessibleThanUnion
{
   private const string _DIAGNOSTIC_ID = "TTRESG077";

   [Fact]
   public async Task Should_trigger_on_public_generic_union_with_internal_member_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            internal class InternalMember;

            [{|#0:Union<InternalMember, string>|}]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InternalMember", "TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_public_non_generic_union_with_internal_member_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            internal class InternalMember;

            [{|#0:AdHocUnion(typeof(InternalMember), typeof(string))|}]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("InternalMember", "TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(AdHocUnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_public_union_with_generic_member_type_having_internal_type_argument()
   {
      var code = """
         #nullable enable
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace TestNamespace
         {
            internal class InternalMember;

            [{|#0:Union<List<InternalMember>, string>|}]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("List<InternalMember>", "TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_union_and_member_type_have_same_accessibility()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            internal class InternalMember;

            [Union<InternalMember, string>]
            internal partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_public_union_is_nested_in_internal_type()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            internal class InternalMember;

            internal partial class Container
            {
               [Union<InternalMember, string>]
               public partial class TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }
}
