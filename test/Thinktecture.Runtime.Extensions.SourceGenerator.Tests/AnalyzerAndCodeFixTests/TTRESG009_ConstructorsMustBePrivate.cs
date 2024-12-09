using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG009_ConstructorsMustBePrivate
{
   private const string _DIAGNOSTIC_ID = "TTRESG009";

   public class Enum_constructors_must_be_private
   {
      [Fact]
      public async Task Should_trigger_if_constructor_is_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_constructor_is_private_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  private protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_constructor_is_private()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
            
                  private {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
      }
   }

   public class Union_class_constructors_must_be_private
   {
      [Fact]
      public async Task Should_trigger_if_constructor_is_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial class TestEnum
            	{
                  protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_constructor_is_private_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial class TestEnum
            	{
                  private protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_constructor_is_private()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial class TestEnum
            	{
                  private {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly });
      }
   }

   public class Union_record_constructors_must_be_private
   {
      [Fact]
      public async Task Should_trigger_if_constructor_is_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial record TestEnum
            	{
                  protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_trigger_if_constructor_is_private_protected()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial record TestEnum
            	{
                  private protected {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_if_constructor_is_private()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
            	public partial record TestEnum
            	{
                  private {|#0:TestEnum|}()
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(UnionAttribute).Assembly });
      }
   }
}
