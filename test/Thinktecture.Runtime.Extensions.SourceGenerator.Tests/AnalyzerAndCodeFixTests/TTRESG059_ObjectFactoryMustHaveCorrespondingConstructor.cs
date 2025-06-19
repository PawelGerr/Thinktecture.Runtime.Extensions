using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG059_ObjectFactoryMustHaveCorrespondingConstructor
{
   private const string _DIAGNOSTIC_ID = "TTRESG059";

   [Fact]
   public async Task Should_trigger_if_no_corresponding_constructor_exists()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(HasCorrespondingConstructor = true)]
            public partial class {|#0:TestClass|};
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithSpan(7, 25, 7, 34).WithArguments("TestClass", "int");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_corresponding_constructor_exists()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(HasCorrespondingConstructor = true)]
            public partial class {|#0:TestClass|}
            {
               public TestClass(int value) { }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_if_HasCorrespondingConstructor_is_false()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(HasCorrespondingConstructor = false)]
            public partial class {|#0:TestClass|};
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_if_HasCorrespondingConstructor_is_not_defined()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            public partial class {|#0:TestClass|};
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
}
