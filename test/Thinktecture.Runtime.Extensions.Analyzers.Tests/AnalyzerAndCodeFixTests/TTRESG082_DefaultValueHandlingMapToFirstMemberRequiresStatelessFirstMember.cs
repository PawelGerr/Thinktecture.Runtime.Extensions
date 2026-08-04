using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG082_DefaultValueHandlingMapToFirstMemberRequiresStatelessFirstMember
{
   private const string _DIAGNOSTIC_ID = "TTRESG082";

   [Fact]
   public async Task Should_trigger_when_first_member_is_not_stateless()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<string, int>({|#0:DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember|})]
            public partial struct TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_first_member_is_stateless()
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
   public async Task Should_not_trigger_when_setting_is_Disallow_even_if_first_member_is_not_stateless()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<string, int>(DefaultValueHandling = UnionDefaultValueHandling.Disallow)]
            public partial struct TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }
}
