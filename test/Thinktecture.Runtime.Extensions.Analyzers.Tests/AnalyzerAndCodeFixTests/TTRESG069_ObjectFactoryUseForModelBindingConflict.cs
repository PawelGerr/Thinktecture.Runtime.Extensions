using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable once InconsistentNaming
public class TTRESG069_ObjectFactoryUseForModelBindingConflict
{
   private const string _DIAGNOSTIC_ID = "TTRESG069";

   [Fact]
   public async Task Should_trigger_when_two_attributes_have_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForModelBinding = true)]
            [ObjectFactory<string>(UseForModelBinding = true)]
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
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_when_three_attributes_have_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForModelBinding = true)]
            [ObjectFactory<string>(UseForModelBinding = true)]
            [ObjectFactory<Guid>(UseForModelBinding = true)]
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
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("TestClass");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(ObjectFactoryAttribute<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_when_only_one_attribute_has_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForModelBinding = true)]
            [ObjectFactory<string>(UseForModelBinding = false)]
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
   public async Task Should_not_trigger_when_no_attributes_have_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForModelBinding = false)]
            [ObjectFactory<string>(UseForModelBinding = false)]
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
   public async Task Should_not_trigger_for_single_attribute_with_UseForModelBinding_true()
   {
      var code = """
         #nullable enable
         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [ObjectFactory<int>(UseForModelBinding = true)]
            public partial class TestClass
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestClass? item)
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
   public async Task Should_not_trigger_when_UseForModelBinding_not_explicitly_set()
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
