using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG109_EmptyStringInFactoryMethodsYieldsNullHasNoEffectOnStructs
{
   private const string _DIAGNOSTIC_ID = "TTRESG109";

   [Fact]
   public async Task Should_trigger_on_struct_with_string_key()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>({|#0:EmptyStringInFactoryMethodsYieldsNull = true|})]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public readonly partial struct TestValueObject;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_struct_with_non_string_key()
   {
      // The setting is ignored on structs in every configuration, so the key type does not matter.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>({|#0:EmptyStringInFactoryMethodsYieldsNull = true|})]
            public readonly partial struct TestValueObject;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_class()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = true)]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public partial class TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_struct_without_the_setting()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public readonly partial struct TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_struct_with_the_setting_disabled()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<string>(EmptyStringInFactoryMethodsYieldsNull = false)]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            [KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
            public readonly partial struct TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }
}
