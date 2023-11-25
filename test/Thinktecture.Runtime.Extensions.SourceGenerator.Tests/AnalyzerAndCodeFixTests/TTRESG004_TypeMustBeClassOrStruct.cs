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
                    	public sealed partial record {|#0:TestEnum|}
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly }, expected);
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
                    	public sealed partial class TestEnum
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
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
                    	public readonly partial struct TestEnum
                    	{
                          public static readonly TestEnum Item1 = default;
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(IEnum<>).Assembly });
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
                    	[ValueObject<string>]
                        public sealed partial record {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_class()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                    	[ValueObject<string>]
                       public sealed partial class TestValueObject
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }

      [Fact]
      public async Task Should_not_trigger_on_struct()
      {
         var code = """

                    using System;
                    using Thinktecture;

                    namespace TestNamespace
                    {
                    	[ValueObject<string>]
                    	public readonly partial struct TestValueObject
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
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
                       public sealed partial record {|#0:TestValueObject|}
                    	{
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly }, expected);
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
                       public sealed partial class TestValueObject
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
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
                    	public readonly partial struct TestValueObject
                    	{
                       }
                    }
                    """;

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly });
      }
   }
}
