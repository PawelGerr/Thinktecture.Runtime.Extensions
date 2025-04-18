using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG037_EnumWithoutDerivedTypesMustBeSealed
{
   private const string _DIAGNOSTIC_ID = "TTRESG037";

   [Fact]
   public async Task Should_trigger_on_inner_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               private class Type_1 : TestEnum
               {
                  public class {|#0:Type_3|} : Type_1
                  {
                  }

                  public sealed class Type_4 : Type_2
                  {
                  }
               }

               private class Type_2 : Type_1
               {
               }
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               private class Type_1 : TestEnum
               {
                  public sealed class Type_3 : Type_1
                  {
                  }

                  public sealed class Type_4 : Type_2
                  {
                  }
               }

               private class Type_2 : Type_1
               {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Type_3");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_sibling_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               private class Type_1 : TestEnum
               {
                  public sealed class Type_3 : Type_1
                  {
                  }

                  public class {|#0:Type_4|} : Type_2
                  {
                  }
               }

               private class Type_2 : Type_1
               {
               }
            }
         }
         """;

      var expectedCode = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default;

               private class Type_1 : TestEnum
               {
                  public sealed class Type_3 : Type_1
                  {
                  }

                  public sealed class Type_4 : Type_2
                  {
                  }
               }

               private class Type_2 : Type_1
               {
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Type_4");
      await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ISmartEnum<>).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_ValidatableEnum_with_derived_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private sealed class DerivedType : TestEnum
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_Enum_with_derived_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private sealed class DerivedType : TestEnum
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_IValidatableEnum_with_generic_derived_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>(IsValidatable = true)]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private sealed class DerivedType<T> : TestEnum
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_Enum_with_generic_derived_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private sealed class DerivedType<T> : TestEnum
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_derived_types_which_are_derived()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private class Type_1 : TestEnum
               {
                  public sealed class Type_3 : Type_1
                  {
                  }

                  public sealed class Type_4 : Type_2
                  {
                  }
               }

               private class Type_2 : Type_1
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_abstract_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public abstract partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_derived_abstract_type()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [SmartEnum<string>]
         	public partial class {|#0:TestEnum|}
         	{
               public static readonly TestEnum Item1 = default;

               private abstract class Type_1 : TestEnum
               {
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(ISmartEnum<>).Assembly]);
   }
}
