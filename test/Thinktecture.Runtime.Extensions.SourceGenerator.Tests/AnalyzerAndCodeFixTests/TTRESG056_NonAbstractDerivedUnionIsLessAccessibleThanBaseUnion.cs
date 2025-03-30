using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public record TTRESG056_NonAbstractDerivedUnionIsLessAccessibleThanBaseUnion
{
   private const string _DIAGNOSTIC_ID = "TTRESG056";

   [Theory]
   [InlineData("class")]
   [InlineData("record")]
   public async Task Should_trigger_on_public_type_with_internal_subtype(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial {{type}} TestUnion
            {
               internal sealed {{type}} {|#0:First|} : TestUnion;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First", "TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
   }

   [Theory]
   [InlineData("class")]
   [InlineData("record")]
   public async Task Should_not_trigger_on_types_with_same_accessibility(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            internal partial {{type}} TestUnion
            {
               internal sealed {{type}} {|#0:First|} : TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   [Theory]
   [InlineData("class")]
   [InlineData("record")]
   public async Task Should_not_trigger_on_type_with_lower_accessibility_than_subtype(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            internal partial {{type}} TestUnion
            {
               public sealed {{type}} {|#0:First|} : TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

}
