using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG081_DefaultValueHandlingMapToFirstMemberRequiresStructUnion
{
   private const string _DIAGNOSTIC_ID = "TTRESG081";

   [Fact]
   public async Task Should_trigger_when_MapToFirstMember_is_used_on_a_class_union()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public struct EmptyState;

            [Union<EmptyState, int>(T1IsStateless = true, {|#0:DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember|})]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_MapToFirstMember_is_used_on_a_struct_union()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public struct EmptyState;

            [Union<EmptyState, int>(T1IsStateless = true, DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember)]
            public partial struct TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_a_class_union_when_setting_is_Disallow()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public struct EmptyState;

            [Union<EmptyState, int>(T1IsStateless = true, DefaultValueHandling = UnionDefaultValueHandling.Disallow)]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }
}
