using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG053_NonAbstractUnionDerivedTypesMustNotBeGeneric
{
   private const string _DIAGNOSTIC_ID = "TTRESG053";

   public class Non_abstract_unions_must_not_be_generic
   {
      [Fact]
      public async Task Should_trigger_on_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion<T>
               {
                  public class {|#0:First|}<T>(T Value) : TestUnion<T>;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First<T>");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_non_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion<T>
               {
                  public class {|#0:First|}(T Value) : TestUnion<T>;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }
}
