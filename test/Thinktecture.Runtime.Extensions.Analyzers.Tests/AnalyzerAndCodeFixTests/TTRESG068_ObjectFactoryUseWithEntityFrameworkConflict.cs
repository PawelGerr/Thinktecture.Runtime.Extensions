using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG068_ObjectFactoryUseWithEntityFrameworkConflict
{
   private const string _DIAGNOSTIC_ID = "TTRESG068";

   [Fact]
   public async Task Should_trigger_when_two_attributes_have_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseWithEntityFramework = true)]
            [ObjectFactory<string>(UseWithEntityFramework = true)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(9, 25, 9, 34).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_trigger_when_three_attributes_have_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseWithEntityFramework = true)]
            [ObjectFactory<string>(UseWithEntityFramework = true)]
            [ObjectFactory<Guid>(UseWithEntityFramework = true)]
            public partial class {|#0:TestClass|}
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
               public string ToValue(int _) => "";
               public Guid ToValue(string _) => Guid.Empty;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected,
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "string"),
         Verifier.Diagnostic("TTRESG062").WithSpan(10, 25, 10, 34).WithArguments("TestClass", "Guid"));
   }

   [Fact]
   public async Task Should_not_trigger_when_only_one_attribute_has_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseWithEntityFramework = true)]
            [ObjectFactory<string>(UseWithEntityFramework = false)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_no_attributes_have_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseWithEntityFramework = false)]
            [ObjectFactory<string>(UseWithEntityFramework = false)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_for_single_attribute_with_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseWithEntityFramework = true)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public int ToValue() => 0;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_when_UseWithEntityFramework_not_explicitly_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>]
            [ObjectFactory<string>]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
}
