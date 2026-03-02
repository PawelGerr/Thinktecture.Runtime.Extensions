using System.Threading.Tasks;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG054_UnionMustBeSealedOrHavePrivateConstructorsOnly
{
   private const string _DIAGNOSTIC_ID = "TTRESG054";

   [Fact]
   public async Task Should_trigger_on_non_sealed_class()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion;
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_trigger_on_class_with_at_least_one_non_private_constructor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }

                  public First(string foo)
                  {
                  }
               }
            }
         }
         """;

      var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("First");
      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
   }

   [Fact]
   public async Task Should_not_trigger_on_sealed_class()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class {|#0:First|} : TestUnion;
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_class_with_private_ctor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public class {|#0:First|} : TestUnion
               {
                  private First()
                  {
                  }
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   [Fact]
   public async Task Should_not_trigger_on_abstract_class_with_private_ctor()
   {
      var code = """

         using System;
         using Thinktecture;

         namespace TestNamespace
         {
            [Union]
            public partial class TestUnion
            {
               public abstract class First : TestUnion
               {
                  private First()
                  {
                  }

                  public sealed class Second : First;
               }
            }
         }
         """;

      await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
   }

   public class DerivedTypes
   {
      [Fact]
      public async Task Should_trigger_on_non_sealed_derived_type_with_public_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     public DerivedUnion() { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_sealed_derived_type_with_protected_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     protected DerivedUnion() { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_sealed_derived_type_with_internal_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     internal DerivedUnion() { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_non_sealed_derived_type_with_protected_internal_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     protected internal DerivedUnion() { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_non_sealed_derived_type_with_only_private_constructors()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class DerivedUnion : BaseUnion
                  {
                     private DerivedUnion() { }
                     private DerivedUnion(int value) { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_sealed_derived_type_with_public_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public sealed partial class DerivedUnion : BaseUnion
                  {
                     public DerivedUnion() { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_on_non_sealed_derived_type_with_mix_of_private_and_public_constructors()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     private DerivedUnion() { }
                     public DerivedUnion(int value) { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }
   }

   public class NestedDerivedTypes
   {
      [Fact]
      public async Task Should_trigger_on_non_sealed_nested_derived_type_with_public_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:NestedDerived|} : BaseUnion
                  {
                     public NestedDerived() { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("NestedDerived");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_sealed_nested_derived_type()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public sealed partial class NestedDerived : BaseUnion
                  {
                     public NestedDerived() { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_non_sealed_nested_derived_type_with_private_constructors()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class NestedDerived : BaseUnion
                  {
                     private NestedDerived() { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }

   public class PartialDeclarations
   {
      [Fact]
      public async Task Should_trigger_on_non_sealed_partial_with_public_constructor_in_one_part()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class {|#0:DerivedUnion|} : BaseUnion
                  {
                     private DerivedUnion() { }
                  }
               }

               public partial class BaseUnion
               {
                  public partial class DerivedUnion
                  {
                     public DerivedUnion(int value) { }
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("DerivedUnion");
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_non_sealed_partial_with_only_private_constructors_across_parts()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public partial class DerivedUnion : BaseUnion
                  {
                     private DerivedUnion() { }
                  }
               }

               public partial class BaseUnion
               {
                  public partial class DerivedUnion
                  {
                     private DerivedUnion(int value) { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }

      [Fact]
      public async Task Should_not_trigger_on_sealed_partial_with_public_constructor()
      {
         var code = """

            using System;
            using Thinktecture;

            namespace TestNamespace
            {
               [Union]
               public partial class BaseUnion
               {
                  private BaseUnion() { }

                  public sealed partial class DerivedUnion : BaseUnion
                  {
                     public DerivedUnion() { }
                  }
               }

               public partial class BaseUnion
               {
                  public sealed partial class DerivedUnion
                  {
                     public DerivedUnion(int value) { }
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly]);
      }
   }
}
