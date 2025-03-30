using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG053_NonAbstractUnionDerivedTypesMustNotBeGeneric
{
   private const string _DIAGNOSTIC_ID = "TTRESG053";

   public class Non_abstract_unions_must_not_be_generic
   {
      [Theory]
      [InlineData("class")]
      [InlineData("record")]
      public async Task Should_trigger_on_generic_class(string type)
      {
         var code = $$"""

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial {{type}} TestUnion<T>
               {
                  public sealed {{type}} {|#0:First|}<T>(T Value) : TestUnion<T>;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First<T>");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_generic_abstract_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion<T>
               {
                  public abstract class {|#0:First|}<T> : TestUnion<T>
                  {
                     private First(T Value)
                     {
                     }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First<T>");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Theory]
      [InlineData("class")]
      [InlineData("record")]
      public async Task Should_not_trigger_on_non_generic_class(string type)
      {
         var code = $$"""

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial {{type}} TestUnion<T>
               {
                  public sealed {{type}} {|#0:First|}(T Value) : TestUnion<T>;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }
}
