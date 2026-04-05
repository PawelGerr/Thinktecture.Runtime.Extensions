using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG074_TypeParamRefRequiresNotnullConstraint
{
   private const string _DIAGNOSTIC_ID = "TTRESG074";

   [Fact]
   public async Task Should_trigger_on_SmartEnum_with_unconstrained_TypeParamRef_and_apply_code_fix()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class TestEnum<T> where T : notnull
             {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("T", "TestEnum<T>");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_SmartEnum_with_notnull_TypeParamRef()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
               where T : notnull
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_SmartEnum_with_struct_TypeParamRef()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
               where T : struct
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_SmartEnum_with_class_TypeParamRef()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
               where T : class
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_on_ValueObject_with_unconstrained_TypeParamRef_and_apply_code_fix()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<TypeParamRef1>]
            public partial class {|#0:TestValueObject|}<T>
            {
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<TypeParamRef1>]
            public partial class TestValueObject<T> where T : notnull
             {
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("T", "TestValueObject<T>");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_ValueObject_with_notnull_TypeParamRef()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<TypeParamRef1>]
            public partial class {|#0:TestValueObject|}<T>
               where T : notnull
            {
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_SmartEnum_with_non_TypeParamRef_key()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<int>]
            public partial class {|#0:TestEnum|}
            {
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_SmartEnum_with_non_nullable_interface_constraint()
   {
      var code = """
         #nullable enable

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface ITestInterface;

            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
               where T : ITestInterface
            {
               public static readonly TestEnum<T> Item1 = default!;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_on_SmartEnum_with_new_constraint_and_merge_where_clause()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class {|#0:TestEnum|}<T>
               where T : new()
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<TypeParamRef1>]
            public partial class TestEnum<T>
               where T : notnull, new()
            {
               public static readonly TestEnum<T> Item1 = default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("T", "TestEnum<T>");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }
}
