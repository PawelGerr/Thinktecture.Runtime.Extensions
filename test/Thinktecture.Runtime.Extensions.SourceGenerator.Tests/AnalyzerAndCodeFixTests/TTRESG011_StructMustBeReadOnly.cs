using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG011_StructMustBeReadOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG011";

   public class Enum_struct_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_if_struct_is_not_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public partial struct {|#0:TestEnum|}
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         var expectedCode = """

                            using System;
                            using Thinktecture;

                            namespace TestNamespace
                            {
                               [SmartEnum<string>(IsValidatable = true)]
                            	public readonly partial struct {|#0:TestEnum|}
                            	{
                                  public static readonly TestEnum Item1 = default;
                               }
                            }
                            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_struct_is_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [SmartEnum<string>(IsValidatable = true)]
                    	public readonly partial struct {|#0:TestEnum|}
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class KeyedValueObject_struct_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_if_struct_is_not_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public partial struct {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         var expectedCode = """

                            using System;
                            using Thinktecture;

                            namespace TestNamespace
                            {
                               [ValueObject<string>]
                            	public readonly partial struct {|#0:TestValueObject|}
                            	{
                               }
                            }
                            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_struct_is_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ValueObject<string>]
                    	public readonly partial struct {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }

   public class ComplexValueObject_struct_must_be_readonly
   {
      [Fact]
      public async Task Should_trigger_if_struct_is_not_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public partial struct {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         var expectedCode = """

                            using System;
                            using Thinktecture;

                            namespace TestNamespace
                            {
                               [ComplexValueObject]
                            	public readonly partial struct {|#0:TestValueObject|}
                            	{
                               }
                            }
                            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_struct_is_readonly()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                       [ComplexValueObject]
                    	public readonly partial struct {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }
}
