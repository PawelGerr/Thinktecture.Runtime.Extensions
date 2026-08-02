using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG108_RefStructObjectFactoryIgnoredBySerializationFrameworks
{
   private const string _DIAGNOSTIC_ID = "TTRESG108";

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_trigger_when_span_factory_is_used_for_MessagePack()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForSerialization = SerializationFrameworks.MessagePack|})]
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

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "ReadOnlySpan<char>", "MessagePack");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_name_only_the_ignored_frameworks_when_span_factory_is_used_for_all_frameworks()
   {
      // System.Text.Json is missing on purpose: it is the supported zero-allocation path for 'ReadOnlySpan<char>'.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForSerialization = SerializationFrameworks.All|})]
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

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "ReadOnlySpan<char>", "NewtonsoftJson, MessagePack");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_SystemTextJson_when_value_type_is_a_non_char_ref_struct()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<ReadOnlySpan<byte>>({|#0:UseForSerialization = SerializationFrameworks.SystemTextJson|})]
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

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass", "ReadOnlySpan<byte>", "SystemTextJson");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_keyless_SmartEnum_owner()
   {
      // MessagePack falls through to the dynamic object resolver because the factory cannot be used.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum]
            [ObjectFactory<ReadOnlySpan<char>>({|#0:UseForSerialization = SerializationFrameworks.MessagePack|})]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = new();
               public static readonly TestEnum Item2 = new();

               private TestEnum()
               {
               }

               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = null;
                  return null;
               }

               public ReadOnlySpan<char> ToValue() => default;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestEnum", "ReadOnlySpan<char>", "MessagePack");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_span_factory_is_used_for_SystemTextJson_only()
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
   public async Task Should_not_trigger_when_value_type_is_not_a_ref_struct()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
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
