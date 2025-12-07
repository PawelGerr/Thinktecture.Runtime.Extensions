using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG106_InnerTypeDoesNotDeriveFromUnion
{
   private const string _DIAGNOSTIC_ID = "TTRESG106";

   public class Inner_type_does_not_derive_from_union
   {
      [Theory]
      [InlineData("class")]
      [InlineData("record")]
      public async Task Should_trigger_on_non_derived_inner_class(string type)
      {
         var code = $$"""

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial {{type}} TestUnion
               {
                  public sealed {{type}} First : TestUnion;
                  public sealed {{type}} {|#0:Second|};
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Second", "TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_multiple_non_derived_inner_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion
               {
                  public sealed class First : TestUnion;
                  public sealed class {|#0:Second|};
                  public sealed class {|#1:Third|};
               }
            }
            """;

         var expected0 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Second", "TestUnion");
         var expected1 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("Third", "TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected0, expected1);
      }

      [Fact]
      public async Task Should_not_trigger_when_all_inner_types_derive_from_union()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion
               {
                  public sealed class First : TestUnion;
                  public sealed class Second : TestUnion;
                  public sealed class Third : TestUnion;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_when_union_has_no_inner_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion
               {
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_nested_type_that_derives_transitively()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion
               {
                  public class First : TestUnion
                  {
                     private First() { }

                     public sealed class Nested : First;
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_enum_or_delegate_inner_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class TestUnion
               {
                  public sealed class First : TestUnion;
                  public enum MyEnum { A, B }
                  public delegate void MyDelegate();
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }
}
