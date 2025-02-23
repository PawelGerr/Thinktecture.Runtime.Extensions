using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG004_TypeMustBeClassOrStruct
{
   private const string _DIAGNOSTIC_ID = "TTRESG004";

   public class Enum_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial record {|#0:TestEnum|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
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
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<string>(IsValidatable = true)]
            	public partial struct TestEnum
            	{
                  public static readonly TestEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(IEnum<>).Assembly]);
      }
   }

   public class KeyedValueObject_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ValueObject<int>]
                public partial record {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ValueObject<int>]
               public partial class TestValueObject
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ValueObject<int>]
            	public partial struct TestValueObject
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class ComplexValueObject_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ComplexValueObject]
               public partial record {|#0:TestValueObject|}
            	{
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ComplexValueObject]
               public partial class TestValueObject
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
            	[ComplexValueObject]
            	public partial struct TestValueObject
            	{
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }

   public class Union_must_be_class_or_struct
   {
      [Fact]
      public async Task Should_trigger_on_record()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union<string, int>]
               public partial record {|#0:TestUnion|};
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
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
      public async Task Should_not_trigger_on_struct()
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
   }
}
