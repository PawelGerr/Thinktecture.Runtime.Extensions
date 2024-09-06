using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG047_TypeMustBeClassOrStruct
{
   private const string _DIAGNOSTIC_ID = "TTRESG047";

   public class Union_must_be_class
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union<string, int>]
                       public partial record {|#0:TestUnion|};
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute<,>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [Union<string, int>]
                       public partial class {|#0:TestUnion|};
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute<,>).Assembly });
      }
   }
}
