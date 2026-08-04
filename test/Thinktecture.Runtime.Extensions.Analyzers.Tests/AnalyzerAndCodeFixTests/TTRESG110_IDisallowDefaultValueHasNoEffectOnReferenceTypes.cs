using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG110_IDisallowDefaultValueHasNoEffectOnReferenceTypes
{
   private const string _DIAGNOSTIC_ID = "TTRESG110";

   [Fact]
   public async Task Should_trigger_on_class_implementing_the_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class TestClass : {|#0:IDisallowDefaultValue|};
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(IDisallowDefaultValue).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_record_implementing_the_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public record TestRecord : {|#0:IDisallowDefaultValue|};
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestRecord");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(IDisallowDefaultValue).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_struct_implementing_the_interface()
   {
      // A hand-written struct opting into the manual handling is the intended use of the now-public interface.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public struct TestStruct : IDisallowDefaultValue;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(IDisallowDefaultValue).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_readonly_struct_implementing_the_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public readonly struct TestStruct : IDisallowDefaultValue;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(IDisallowDefaultValue).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_keyed_struct_value_object_that_gets_the_interface_from_the_generator()
   {
      // The source generator emits the interface on the generated partial declaration. That emission must not
      // be treated as a manual implementation, so neither TTRESG110 nor TTRESG080 is reported here.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial struct TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ValueObjectAttribute<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_class_without_the_interface()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public class TestClass;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(IDisallowDefaultValue).Assembly]);
   }
}
