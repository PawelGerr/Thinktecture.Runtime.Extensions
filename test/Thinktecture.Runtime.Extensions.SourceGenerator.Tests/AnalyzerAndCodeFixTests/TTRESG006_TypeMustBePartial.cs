using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG006_TypeMustBePartial
{
   private const string _DIAGNOSTIC_ID = "TTRESG006";

   public class Enum_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public class {|#0:TestEnum|}
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
            	public partial class {|#0:TestEnum|}
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public struct {|#0:TestEnum|}
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
            	public partial struct {|#0:TestEnum|}
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial class {|#0:TestEnum|}
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
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

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }
   }

   public class KeyedValueObject_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public class {|#0:TestValueObject|}
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
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public struct {|#0:TestValueObject|}
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
            	public partial struct {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ValueObject<string>]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
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

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class ComplexValueObject_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public class {|#0:TestValueObject|}
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
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public struct {|#0:TestValueObject|}
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
            	public partial struct {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [ComplexValueObject]
            	public partial class {|#0:TestValueObject|}
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
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

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class AdHocUnion_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public class {|#0:TestUnion|};
            }
            """;

         var expectedCode = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public partial class {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union<string, int>]
               public partial class {|#0:TestUnion|};
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public struct {|#0:TestUnion|};
            }
            """;

         var expectedCode = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public partial struct {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union<string, int>]
               public partial struct {|#0:TestUnion|};
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_ref_struct()
      {
         var code = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public ref struct {|#0:TestUnion|};
            }
            """;

         var expectedCode = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union<string, int>]
            	   public ref partial struct {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_ref_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union<string, int>]
               public ref partial struct {|#0:TestUnion|};
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
      }
   }

   public class Union_must_be_partial
   {
      [Fact]
      public async Task Should_trigger_on_non_partial_class()
      {
         var code = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union]
            	   public class {|#0:TestUnion|};
            }
            """;

         var expectedCode = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union]
            	   public partial class {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class {|#0:TestUnion|};
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_non_partial_struct()
      {
         var code = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union]
            	   public record {|#0:TestUnion|};
            }
            """;

         var expectedCode = """
            using System;
            using Thinktecture;

            namespace TestNamespace
            {
                [Union]
            	   public partial record {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_partial_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial record {|#0:TestUnion|};
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }
}
