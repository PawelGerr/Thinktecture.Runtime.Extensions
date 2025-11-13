using System.Threading.Tasks;
using Thinktecture.Internal;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.AnalyzerVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsInternalUsageAnalyzer>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG1000_InternalApiUsage_EdgeCases
{
   private const string _DIAGNOSTIC_ID = "TTRESG1000";

   [Fact]
   public async Task Should_trigger_on_object_creation()
   {
      var code = """
         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public void TestMethod()
            {
               var m = {|#0:typeof(Metadata)|};
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_only_once_for_multiple_variable_declarators()
   {
      var code = """
         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public void TestMethod()
            {
               {|#0:Metadata|} m1 = null, m2 = null; // should report only once
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_each_internal_parameter()
   {
      var code = """
         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public void TestMethod({|#0:Metadata|} m1, {|#1:Metadata|} m2)
            {
            }
         }
         """;

      var expected = new[]
                     {
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata"),
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("Thinktecture.Internal.Metadata"),
                     };
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_each_internal_generic_type_argument()
   {
      var code = """
         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public void TestMethod()
            {
               GenericMethod<{|#0:Metadata|}, {|#1:Metadata|}>();
            }

            public void GenericMethod<T1, T2>() { }
         }
         """;

      var expected = new[]
                     {
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata"),
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("Thinktecture.Internal.Metadata"),
                     };
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_multiple_internal_interfaces()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace;

         [ThinktectureRuntimeExtensionInternal]
         public interface IInt1 { }
         [ThinktectureRuntimeExtensionInternal]
         public interface IInt2 { }

         public class {|#0:TestClass|} : IInt1, IInt2 { }
         """;

      var expected = new[]
                     {
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestNamespace.IInt1"),
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestNamespace.IInt2"),
                     };
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ThinktectureRuntimeExtensionInternalAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_type_with_internal_attribute_used_in_property()
   {
      var code = """
         using System;
         using Thinktecture;

         namespace TestNamespace;

         [ThinktectureRuntimeExtensionInternal]
         public class InternalType { }

         public class TestClass
         {
            public {|#0:InternalType|} Prop { get; set; }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestNamespace.InternalType");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ThinktectureRuntimeExtensionInternalAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_for_multiple_internal_generic_constraints()
   {
      var code = """
         using System;
         using Thinktecture.Internal;
         using Thinktecture;

         namespace TestNamespace;

         [ThinktectureRuntimeExtensionInternal]
         public interface IInternalType2 { }

         public class TestClass
         {
            public void {|#0:TestMethod|}<T>(T value)
               where T : Metadata, IInternalType2
            {
            }
         }
         """;
      var expected = new[]
                     {
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestNamespace.IInternalType2"),
                        Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata"),
                     };

      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }
}
