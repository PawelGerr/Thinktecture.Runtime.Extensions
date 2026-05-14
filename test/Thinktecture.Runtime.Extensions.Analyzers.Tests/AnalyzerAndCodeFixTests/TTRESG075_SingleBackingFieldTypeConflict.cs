using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG075_SingleBackingFieldTypeConflict
{
   private const string _DIAGNOSTIC_ID = "TTRESG075";

   [Fact]
   public async Task Should_diagnose_when_UseSingleBackingField_false_and_SingleBackingFieldType_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface IFoo { string Bar { get; } }
            public class Foo1 : IFoo { public string Bar => "foo1"; }
            public class Foo2 : IFoo { public string Bar => "foo2"; }

            [Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo), UseSingleBackingField = false)]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithSpan(11, 5, 11, 92).WithArguments("TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_diagnose_when_UseSingleBackingField_false_and_SingleBackingFieldType_typeof_object()
   {
      // Per implementer's note: normalization happens after the analyzer, so typeof(object) still triggers the conflict
      // when paired with explicit UseSingleBackingField = false.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, string>(SingleBackingFieldType = typeof(object), UseSingleBackingField = false)]
            public partial class TestUnion;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithSpan(7, 5, 7, 95).WithArguments("TestUnion");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_diagnose_when_UseSingleBackingField_true_and_SingleBackingFieldType_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface IFoo { string Bar { get; } }
            public class Foo1 : IFoo { public string Bar => "foo1"; }
            public class Foo2 : IFoo { public string Bar => "foo2"; }

            [Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo), UseSingleBackingField = true)]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_diagnose_when_SingleBackingFieldType_set_alone()
   {
      // SingleBackingFieldType set without explicit UseSingleBackingField — cascade implies true; no conflict.
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public interface IFoo { string Bar { get; } }
            public class Foo1 : IFoo { public string Bar => "foo1"; }
            public class Foo2 : IFoo { public string Bar => "foo2"; }

            [Union<Foo1, Foo2>(SingleBackingFieldType = typeof(IFoo))]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_diagnose_when_only_UseSingleBackingField_false()
   {
      // No SingleBackingFieldType, just UseSingleBackingField = false — valid (default behavior).
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, string>(UseSingleBackingField = false)]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }

   [Fact]
   public async Task Should_not_diagnose_when_neither_attribute_set()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union<int, string>]
            public partial class TestUnion;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly]);
   }
}
