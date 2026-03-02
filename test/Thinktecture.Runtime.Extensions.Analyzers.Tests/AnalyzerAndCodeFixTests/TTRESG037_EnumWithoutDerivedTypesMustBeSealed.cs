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
   public async Task Should_not_trigger_having_derived_type()
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
   public async Task Should_not_trigger_having_generic_derived_type()
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

   public class SingleLevelHierarchy
   {
      [Fact]
      public async Task Should_trigger_on_leaf_non_sealed_derived_enum()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private sealed class DerivedEnum1 : BaseEnum
                  {
                  }

                  private partial class {|#0:DerivedEnum2|} : BaseEnum
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedEnum2");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_base_enum_with_derived_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private sealed class DerivedEnum : BaseEnum
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_sealed_leaf_enum()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private sealed class DerivedEnum : BaseEnum
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_abstract_derived_enum_with_further_derivations()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class AbstractDerived : BaseEnum
                  {
                  }

                  private sealed class ConcreteDerived : AbstractDerived
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }
   }

   public class MultiLevelHierarchy
   {
      [Fact]
      public async Task Should_trigger_on_multiple_leaf_nodes_that_are_not_sealed()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Level1 : BaseEnum
                  {
                  }

                  private partial class {|#0:Leaf1|} : Level1
                  {
                  }

                  private partial class {|#1:Leaf2|} : Level1
                  {
                  }
               }
            }
            """;

         var expected1 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Leaf1");
         var expected2 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments("Leaf2");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected1, expected2);
      }

      [Fact]
      public async Task Should_trigger_on_deep_leaf_non_sealed_enum()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Level1 : BaseEnum
                  {
                  }

                  private abstract class Level2 : Level1
                  {
                  }

                  private abstract class Level3 : Level2
                  {
                  }

                  private partial class {|#0:LeafEnum|} : Level3
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("LeafEnum");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_intermediate_non_sealed_with_derived_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Level1 : BaseEnum
                  {
                  }

                  private partial class Level2 : Level1
                  {
                  }

                  private sealed class LeafEnum : Level2
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_when_all_leaves_are_sealed()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Branch1 : BaseEnum
                  {
                  }

                  private abstract class Branch2 : BaseEnum
                  {
                  }

                  private sealed class Leaf1 : Branch1
                  {
                  }

                  private sealed class Leaf2 : Branch2
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }
   }

   public class ComplexScenarios
   {
      [Fact]
      public async Task Should_trigger_on_non_sealed_leaf_with_sealed_sibling()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Branch : BaseEnum
                  {
                  }

                  private sealed class SealedLeaf : Branch
                  {
                  }

                  private partial class {|#0:NonSealedLeaf|} : Branch
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("NonSealedLeaf");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_abstract_intermediate_nodes()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Branch1 : BaseEnum
                  {
                  }

                  private abstract class Branch2 : Branch1
                  {
                  }

                  private sealed class Leaf : Branch2
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_non_sealed_non_abstract_intermediate_without_further_derivations()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class Branch1 : BaseEnum
                  {
                  }

                  private partial class {|#0:Branch2|} : Branch1
                  {
                  }

                  private sealed class Leaf : Branch1
                  {
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Branch2");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_when_enum_has_only_abstract_derived_types()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public partial class BaseEnum
               {
                  public static readonly BaseEnum Item1 = default;

                  private abstract class AbstractDerived : BaseEnum
                  {
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_sealed_standalone_enum()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [SmartEnum<int>]
               public sealed partial class StandaloneEnum
               {
                  public static readonly StandaloneEnum Item1 = default;
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(SmartEnumAttribute<>).Assembly]);
      }
   }
}
