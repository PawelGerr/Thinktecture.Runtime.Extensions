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
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseWithEntityFramework = true|})]
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
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForModelBinding = true|})]
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
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseWithEntityFramework = true|}, {|#1:UseForModelBinding = true|})]
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
         Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("TestClass", "UseForModelBinding"));
   }

   [Fact]
   public async Task Should_trigger_when_byte_span_factory_has_UseWithEntityFramework_true()
   {
      // Every ref struct is affected, not only 'ReadOnlySpan<char>'.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<byte>>({|#0:UseWithEntityFramework = true|})]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<byte> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public ReadOnlySpan<byte> ToValue() => default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseWithEntityFramework");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_span_factory_of_a_SmartEnum_has_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForModelBinding = true|})]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new("A");
               public static readonly TestEnum Item2 = new("B");

               private TestEnum(string key)
               {
               }

               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = null;
                  return null;
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "UseForModelBinding");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_report_UseWithEntityFramework_conflict_when_one_factory_is_a_span_factory()
   {
      // The span factory must not carry the flag at all (TTRESG078), so it does not count as a
      // second Entity Framework factory. Reporting TTRESG068 on top would be a double report.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseWithEntityFramework = true|})]
            [ObjectFactory<string>(UseWithEntityFramework = true)]
            public partial class {|#1:TestClass|}
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
               {
                  item = null;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestClass? item)
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
         Verifier.Diagnostic("TTRESG062").WithLocation(1).WithArguments("TestClass", "string"));
   }

   [Fact]
   public async Task Should_not_report_UseForModelBinding_conflict_when_one_factory_is_a_span_factory()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForModelBinding = true|})]
            [ObjectFactory<string>(UseForModelBinding = true)]
            public partial class TestClass
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestClass? item)
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

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "UseForModelBinding");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
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
