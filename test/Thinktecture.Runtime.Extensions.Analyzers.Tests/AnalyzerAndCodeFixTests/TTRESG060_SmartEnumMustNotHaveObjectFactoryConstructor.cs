using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG060_SmartEnumMustNotHaveObjectFactoryConstructor
{
   private const string _DIAGNOSTIC_ID = "TTRESG060";

   [Fact]
   public async Task Should_trigger_if_HasCorrespondingConstructor_is_true_on_SmartEnum_with_ObjectFactory()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
            [ObjectFactory<string>(HasCorrespondingConstructor = true)]
            public partial class {|#0:TestEnum|}
            {
               public static readonly TestEnum Item1 = new("A");
               public static readonly TestEnum Item2 = new("B");
               private TestEnum(string key) { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithSpan(8, 25, 8, 33).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_if_HasCorrespondingConstructor_is_false()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
            [ObjectFactory<string>(HasCorrespondingConstructor = false)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new("A");
               public static readonly TestEnum Item2 = new("B");
               private TestEnum(string key) { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_if_ObjectFactory_is_not_present()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new("A");
               public static readonly TestEnum Item2 = new("B");
               private TestEnum(string key) { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_if_ObjectFactory_is_present_without_HasCorrespondingConstructor()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
            [ObjectFactory<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new("A");
               public static readonly TestEnum Item2 = new("B");
               private TestEnum(string key) { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_for_keyless_SmartEnum_with_ObjectFactory_HasCorrespondingConstructor_true()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum]
            [ObjectFactory<string>(HasCorrespondingConstructor = true)]
            public partial class {|#0:TestEnum|}
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();
               private TestEnum() { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithSpan(8, 25, 8, 33).WithArguments("TestEnum", "string");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_for_keyless_SmartEnum_with_ObjectFactory_HasCorrespondingConstructor_false()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum]
            [ObjectFactory<string>(HasCorrespondingConstructor = false)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();
               private TestEnum() { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_for_keyless_SmartEnum_with_ObjectFactory_without_HasCorrespondingConstructor()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum]
            [ObjectFactory<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();
               private TestEnum() { }

               public static ValidationError Validate(string value, IFormatProvider provider, out TestEnum item) => throw new NotImplementedException();
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
}
