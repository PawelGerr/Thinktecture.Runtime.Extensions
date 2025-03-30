using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public record TTRESG055_UnionRecordMustBeSealed
{
   private const string _DIAGNOSTIC_ID = "TTRESG055";

   [Fact]
   public async Task Should_trigger_on_non_sealed_record()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public record {|#0:First|} : TestUnion;
            }
         }
         """;

      var fixedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record {|#0:First|} : TestUnion;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyCodeFixAsync(code, fixedCode, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_record_with_private_ctor_due_to_copy_constructor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public record {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      var fixedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyCodeFixAsync(code, fixedCode, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_abstract_record_with_private_ctor_due_to_copy_constructor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public abstract record {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      var fixedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyCodeFixAsync(code, fixedCode, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_sealed_record()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial record TestUnion
            {
               public sealed record {|#0:First|} : TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }
}
