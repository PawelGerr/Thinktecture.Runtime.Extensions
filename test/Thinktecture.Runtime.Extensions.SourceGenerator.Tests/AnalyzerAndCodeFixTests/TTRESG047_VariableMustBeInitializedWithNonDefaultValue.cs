using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.TestUnions;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG047_VariableMustBeInitializedWithNonDefaultValue
{
   private const string _DIAGNOSTIC_ID = "TTRESG047";

   [Fact]
   public async Task Should_trigger_on_assignment_with_default()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestUnions;

                 namespace TestNamespace
                 {
                    public class TestClass
                    {
                        public void TestMethod()
                        {
                           TestUnion_struct_string_int testStruct = {|#0:default|};
                        }
                    }
                 }
                 """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_default_ctor()
   {
      var code = """

                 using System;
                 using Thinktecture;
                 using Thinktecture.Runtime.Tests.TestUnions;

                 namespace TestNamespace
                 {
                    public class TestClass
                    {
                        public void TestMethod()
                        {
                           TestUnion_struct_string_int testStruct = {|#0:new()|};
                        }
                    }
                 }
                 """;

      var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion_struct_string_int");
      await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(TestUnion_struct_string_int).Assembly, typeof(UnionAttribute<,>).Assembly], expected);
   }
}
