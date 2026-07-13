using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG078_ReadOnlySpanOfCharObjectFactoryMustNotBeUsedWithEntityFrameworkOrModelBinding
{
   private const string _DIAGNOSTIC_ID = "TTRESG078";

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_trigger_when_span_factory_has_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [{|#0:ObjectFactory<ReadOnlySpan<char>>(UseWithEntityFramework = true)|}]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public ReadOnlySpan<char> ToValue() => default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseWithEntityFramework");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_span_factory_has_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [{|#0:ObjectFactory<ReadOnlySpan<char>>(UseForModelBinding = true)|}]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseForModelBinding");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_twice_when_span_factory_has_both_flags_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [{|#0:ObjectFactory<ReadOnlySpan<char>>(UseWithEntityFramework = true, UseForModelBinding = true)|}]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public ReadOnlySpan<char> ToValue() => default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly],
         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseWithEntityFramework"),
         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseForModelBinding"));
   }

   [Fact]
   public async Task Should_not_trigger_when_span_factory_is_used_only_for_serialization()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public ReadOnlySpan<char> ToValue() => default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
#endif

   [Fact]
   public async Task Should_not_trigger_when_non_span_factory_has_UseWithEntityFramework_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseWithEntityFramework = true)]
            public partial class TestClass
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public string ToValue() => "";
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly]);
   }
}
