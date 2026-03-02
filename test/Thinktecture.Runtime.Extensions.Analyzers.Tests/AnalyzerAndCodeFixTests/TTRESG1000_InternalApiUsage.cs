using System.Threading.Tasks;
using Thinktecture.Internal;
using Thinktecture.Text.Json.Serialization;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.AnalyzerVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsInternalUsageAnalyzer>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG1000_InternalApiUsage
{
   private const string _DIAGNOSTIC_ID = "TTRESG1000";

   [Fact]
   public async Task Should_trigger_on_interface_implementation()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace;

         public class {|#0:TestClass|} : IDisallowDefaultValue;
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.IDisallowDefaultValue");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_base_class()
   {
      var code = """

         using System;
         using Thinktecture;
         using Thinktecture.Internal;
         using Thinktecture.Text.Json.Serialization;

         namespace TestNamespace;

         public class {|#0:TestClass|}<T, TValidationError> : ThinktectureJsonConverterFactory<T, TValidationError>
            where T : IObjectFactory<T, string, TValidationError>, IConvertible<string>
            where TValidationError : class, IValidationError<TValidationError>
         {
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Text.Json.Serialization.ThinktectureJsonConverterFactory<T, TValidationError>");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(IConvertible<>).Assembly, typeof(ThinktectureJsonConverterFactory).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_method_call()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void TestMethod()
         	{
         		{|#0:MetadataLookup.Find|}(null);
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.MetadataLookup");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_variable()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void TestMethod()
         	{
         		{|#0:Metadata|} metadata = null;
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_property()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public {|#0:Metadata|} Metadata { get; set; }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_field()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
            public {|#0:Metadata|} Metadata;
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_method_argument()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void TestMethod({|#0:Metadata|} metadata)
         	{
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_method_return_type()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public {|#0:Metadata|} TestMethod()
         	{
         		return null;
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_method_generic_constraint()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void {|#0:TestMethod|}<T>(T metadata)
         	   where T : Metadata
         	{
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_method_call_with_generic_argument()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void TestMethod()
         	{
         		TestMethod<{|#0:Metadata|}>();
         	}

         	public void TestMethod<T>()
            {
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_typeof()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public Type TestMethod()
         	{
         		return {|#0:typeof(Metadata)|};
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_type_cast()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public object TestMethod(object obj)
         	{
         		return {|#0:(Metadata)obj|};
         	}
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Thinktecture.Internal.Metadata");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(MetadataLookup).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_property_access()
   {
      var code = """

         using System;
         using Thinktecture.Internal;

         namespace TestNamespace;

         public class TestClass
         {
         	public void TestMethod({|#0:Metadata|} metadata)
         	{
         	   var type = {|#1:metadata.Type|};
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
}
