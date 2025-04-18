using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.CodeFixes;
using Thinktecture.CodeAnalysis.Diagnostics;
using Thinktecture.Runtime.Tests.Verifiers;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG033_EnumsValueObjectsAndAdHocUnionsMustNotBeGeneric
{
   private const string _DIAGNOSTIC_ID = "TTRESG033";

   public class Enums_must_not_be_generic
   {
      [Fact]
      public async Task Should_trigger_on_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|}<T>
            	{
                  public static readonly TestEnum<T> Item1 = default;
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum<T>");
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
               [SmartEnum<string>(IsValidatable = true)]
            	public partial struct {|#0:TestEnum|}<T>
            	{
                  public static readonly TestEnum<T> Item1 = default;
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum<T>");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }

   public class KeyedValue_objects_must_not_be_generic
   {
      [Fact]
      public async Task Should_trigger_on_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<int>]
            	public partial class {|#0:TestValueObject|}<T>
            	{
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject<T>");
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
               [ValueObject<int>]
            	public partial struct {|#0:TestValueObject|}<T>
            	{
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject<T>");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }

   public class ComplexValue_objects_must_not_be_generic
   {
      [Fact]
      public async Task Should_trigger_on_generic_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|}<T>
            	{
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject<T>");
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
               [ComplexValueObject]
            	public partial struct {|#0:TestValueObject|}<T>
            	{
               }
            }
            """;

         var expected = CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject<T>");
         await CodeFixVerifier<ThinktectureRuntimeExtensionsAnalyzer, ThinktectureRuntimeExtensionsCodeFixProvider>.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly], expected);
      }
   }

   public class Union_must_not_be_generic
   {
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
}
