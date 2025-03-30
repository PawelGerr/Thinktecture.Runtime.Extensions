using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG054_UnionMustBeSealedOrHavePrivateConstructorsOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG054";

   [Fact]
   public async Task Should_trigger_on_non_sealed_class()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_class_with_at_least_one_non_private_constructor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }

                  public First(string foo)
                  {
                  }
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_sealed_class()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class {|#0:First|} : TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_class_with_private_ctor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_abstract_class_with_private_ctor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public abstract class First : TestUnion
               {
                  private First()
                  {
                  }

                  public sealed class Second : First;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }
}
