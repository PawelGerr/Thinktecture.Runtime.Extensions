using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG033_AdHocUnionsMustNotBeGeneric
{
   private const string _DIAGNOSTIC_ID = "TTRESG033";

   [Fact]
   public async Task Should_trigger_on_generic_class()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<string, int>]
            public partial class {|#0:TestUnion|}<T>;
         }
         """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion<T>");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_generic_struct()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<string, int>]
            public partial struct {|#0:TestUnion|}<T>;
         }
         """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion<T>");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
   }
}
