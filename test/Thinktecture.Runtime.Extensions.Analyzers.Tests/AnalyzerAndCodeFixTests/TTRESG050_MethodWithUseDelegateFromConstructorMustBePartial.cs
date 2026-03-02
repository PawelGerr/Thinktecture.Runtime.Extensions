using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG050_MethodWithUseDelegateFromConstructorMustBePartial
{
   private const string _DIAGNOSTIC_ID = "TTRESG050";

   [Fact]
   public async Task Should_trigger_on_non_partial_method()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               public void {|#0:Do|}();

               private static void DoItem1() { }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Do");
      var toIgnore = Verifier.Diagnostic("CS0501", "'TestEnum.Do()' must declare a body because it is not marked abstract, extern, or partial").WithSpan(12, 19, 12, 21).WithArguments("TestNamespace.TestEnum.Do()");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly], [toIgnore, expected]);
   }

   [Fact]
   public async Task Should_not_trigger_on_partial_method()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               partial void Do();

               private static void DoItem1() { }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_work_with_methods_that_have_return_types()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               public string {|#0:Get|}();

               private static string GetItem1() => "Item1";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Get");
      var toIgnore = Verifier.Diagnostic("CS0501", "'TestEnum.Get()' must declare a body because it is not marked abstract, extern, or partial").WithSpan(12, 21, 12, 24).WithArguments("TestNamespace.TestEnum.Get()");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly], [toIgnore, expected]);
   }

   [Fact]
   public async Task Should_work_with_methods_that_have_parameters()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default;

               [UseDelegateFromConstructor]
               public void {|#0:Do|}(int value);

               private static void ProcessItem1(int value) { }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Do");
      var toIgnore = Verifier.Diagnostic("CS0501", "'TestEnum.Do(int)' must declare a body because it is not marked abstract, extern, or partial").WithSpan(12, 19, 12, 21).WithArguments("TestNamespace.TestEnum.Do(int)");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UseDelegateFromConstructorAttribute).Assembly], [toIgnore, expected]);
   }
}
