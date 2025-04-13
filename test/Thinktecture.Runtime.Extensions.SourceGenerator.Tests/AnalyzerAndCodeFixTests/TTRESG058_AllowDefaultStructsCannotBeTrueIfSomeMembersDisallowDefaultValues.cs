using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public record TTRESG058_AllowDefaultStructsCannotBeTrueIfSomeMembersDisallowDefaultValues
{
   private const string _DIAGNOSTIC_ID = "TTRESG058";

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   public async Task Should_not_trigger_on_empty_complex_value_object(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ComplexValueObject(AllowDefaultStructs = true)]
            public partial {{type}} TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   public async Task Should_not_trigger_on_complex_value_object_with_non_value_object_member(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public record TestClass;

            [ComplexValueObject(
               AllowDefaultStructs = true,
               DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            public partial {{type}} TestValueObject
            {
               public int IntMember { get; }
               public int? NullableIntMember { get; }
               public string StringMember { get; }
               public TestClass ClassMember { get; }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   public async Task Should_not_trigger_on_complex_value_object_with_value_object_member_that_allow_default_values(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;

         namespace TestNamespace
         {
            public record TestClass;

            [ComplexValueObject(
               AllowDefaultStructs = true,
               DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            public partial {{type}} TestValueObject
            {
               public IntBasedStructValueObject IntBasedStructValueObjectMember { get; } // AllowDefaultStructs = true
               public StringBasedStructValueObject? StringBasedStructValueObjectMember { get; } // AllowDefaultStructs = false, but it is a Nullable<T>
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(StringBasedStructValueObject).Assembly]);
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   public async Task Should_trigger_on_complex_value_object_with_value_object_member_that_dont_allow_default_values(string type)
   {
      var code = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;

         namespace TestNamespace
         {
            [ValueObject<int>]
            public partial struct OtherValueObject;

            [ComplexValueObject(
               AllowDefaultStructs = true,
               DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            public partial {{type}} {|#0:TestValueObject|}
            {
               public StringBasedStructValueObject StringBasedStructValueObjectMember { get; } // AllowDefaultStructs = false
               public OtherValueObject OtherValueObjectMember { get; } // AllowDefaultStructs = false
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "StringBasedStructValueObjectMember, OtherValueObjectMember");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(StringBasedStructValueObject).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_keyed_value_object_with_non_value_object_member()
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ValueObject<int>(AllowDefaultStructs = true)]
            public partial class TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_keyed_value_object_with_member_that_allow_default_values()
   {
      var code = $$"""

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            public partial struct OtherValueObject;

            [ValueObject<OtherValueObject>(AllowDefaultStructs = true)]
            public partial class TestValueObject;
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_trigger_on_keyed_value_object_with_member_that_dont_allow_default_values()
   {
      var code = $$"""

         using System;
         using Thinktecture;
         using Thinktecture.Runtime.Tests.TestValueObjects;

         namespace TestNamespace
         {
            public partial struct OtherValueObject : IDisallowDefaultValue;

            [ValueObject<OtherValueObject>(AllowDefaultStructs = true)]
            public partial class {|#0:TestValueObject|};
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestValueObject", "OtherValueObject");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(StringBasedStructValueObject).Assembly], expected);
   }
}
