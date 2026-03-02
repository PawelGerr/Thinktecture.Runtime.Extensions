using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG1001_UseSwitchMapWithStaticLambda
{
   private const string _DIAGNOSTIC_ID = "TTRESG1001";

   public class SmartEnum
   {
      public class SwitchWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(item1: () => {}, item2: () => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        testEnum.SwitchPartially(@default: static value => {}, item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        testEnum.{|#0:SwitchPartially|}(@default: value => {}, item1: () => {}, item2: () => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        testEnum.SwitchPartially(@default: static value => {}, item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("SwitchPartially");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.Switch(item1: static () => 1, item2: static () => 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.{|#0:Switch|}(item1: () => 1, item2: () => 2);
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.Switch(item1: static () => 1, item2: static () => 2);
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class SwitchWithActionAndState
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(42, item1: static state => {}, item2: static state => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(42, item1: state => {}, item2: state => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(42, item1: static state => {}, item2: static state => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class SwitchWithFuncAndState
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.Switch(42, item1: static state => 1, item2: static state => 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.{|#0:Switch|}(42, item1: state => 1, item2: state => 2);
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.Switch(42, item1: static state => 1, item2: static state => 2);
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_not_trigger_on_map()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        var result = testEnum.Map(item1: 1, item2: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class MapPartially
      {
         [Fact]
         public async Task Should_not_trigger_on_map_partially()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        var result = testEnum.MapPartially(@default: -1, item1: 1, item2: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly]);
         }
      }
   }

   public class AdHocUnion
   {
      public class SwitchWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.Switch(@string: static s => {}, int32: static i => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.{|#0:Switch|}(@string: s => {}, int32: i => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.Switch(@string: static s => {}, int32: static i => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.SwitchPartially(@default: static item => {}, @string: static s => {}, int32: static i => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.{|#0:SwitchPartially|}(@default: item => {}, @string: s => {}, int32: i => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        testUnion.SwitchPartially(@default: static item => {}, @string: static s => {}, int32: static i => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("SwitchPartially");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        var result = testUnion.Switch(@string: static s => s.Length, int32: static i => i);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        var result = testUnion.{|#0:Switch|}(@string: s => s.Length, int32: i => i);
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        var result = testUnion.Switch(@string: static s => s.Length, int32: static i => i);
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_not_trigger_on_map()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        var result = testUnion.Map(@string: 1, int32: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class MapPartially
      {
         [Fact]
         public async Task Should_not_trigger_on_map_partially()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestAdHocUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion_class_string_int("text");

                        var result = testUnion.MapPartially(@default: 0, @string: 1, int32: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }
   }

   public class Union
   {
      public class SwitchWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.Switch(child1: static c => {}, child2: static c => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.{|#0:Switch|}(child1: c => {}, child2: c => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.Switch(child1: static c => {}, child2: static c => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.SwitchPartially(@default: static d => {}, child1: static c => {}, child2: static c => {});
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.{|#0:SwitchPartially|}(@default: d => {}, child1: c => {}, child2: c => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");

                        testUnion.SwitchPartially(@default: static d => {}, child1: static c => {}, child2: static c => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("SwitchPartially");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_not_trigger_on_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        TestUnion testUnion = new TestUnion.Child1("text");

                        var result = testUnion.Switch(child1: static c => 1, child2: static c => 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }

         [Fact]
         public async Task Should_make_lambda_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        TestUnion testUnion = new TestUnion.Child1("text");

                        var result = testUnion.{|#0:Switch|}(child1: c => 1, child2: c => 2);
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        TestUnion testUnion = new TestUnion.Child1("text");

                        var result = testUnion.Switch(child1: static c => 1, child2: static c => 2);
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_not_trigger_on_map()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        TestUnion testUnion = new TestUnion.Child1("text");

                        var result = testUnion.Map(child1: 1, child2: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class MapPartially
      {
         [Fact]
         public async Task Should_not_trigger_on_map_partially()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        TestUnion testUnion = new TestUnion.Child1("text");

                        var result = testUnion.MapPartially(@default: 0, child1: 1, child2: 2);
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }
   }

   public class EdgeCases
   {
      public class MixedStaticAndNonStaticLambdas
      {
         [Fact]
         public async Task Should_trigger_when_only_some_lambdas_are_static()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(item1: static () => {}, item2: () => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_when_method_group_mixed_with_non_static_lambda()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(item1: HandleItem, item2: () => {});
                     }

                     private static void HandleItem() { }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(item1: HandleItem, item2: static () => {});
                     }

                     private static void HandleItem() { }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class MethodGroups
      {
         [Fact]
         public async Task Should_not_trigger_on_method_groups()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(item1: HandleItem, item2: HandleItem);
                     }

                     private static void HandleItem() { }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class ClosureWithSingleCapture
      {
         [Fact]
         public async Task Should_convert_to_state_overload_with_single_capture()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;
                        var x = 42;

                        testEnum.{|#0:Switch|}(item1: () => { _ = x; }, item2: () => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;
                        var x = 42;

                        testEnum.Switch(state: x, item1: static state => { _ = state; }, item2: static state => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class ClosureWithMultipleCaptures
      {
         [Fact]
         public async Task Should_convert_to_state_overload_with_tuple_for_multiple_captures()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;
                        var x = 42;
                        var y = 43;

                        testEnum.{|#0:Switch|}(item1: () => { _ = x; }, item2: () => { _ = y; });
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;
                        var x = 42;
                        var y = 43;

                        testEnum.Switch(state: (x, y), item1: static state => { _ = state.x; }, item2: static state => { _ = state.y; });
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class ClosureCapturingThis
      {
         [Fact]
         public async Task Should_trigger_when_capturing_this()
         {
            // When a lambda captures 'this' (via instance field access), TryGetCapturedVariables returns false,
            // so the code fix falls back to Strategy B (simple static addition).
            // We only verify the diagnostic fires here because the resulting code with 'static' on a lambda
            // that accesses instance members would not compile.
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     private int _field = 42;

                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(item1: () => { _ = _field; }, item2: () => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class AlreadyUsingStateOverload
      {
         [Fact]
         public async Task Should_make_lambda_static_without_state_conversion_when_state_overload_already_used()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.{|#0:Switch|}(42, item1: state => {}, item2: state => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased.Item1;

                        testEnum.Switch(42, item1: static state => {}, item2: static state => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("Switch");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }
      }

      public class SwitchPartiallyWithDefaultOmitted
      {
         [Fact]
         public async Task Should_trigger_when_default_is_omitted()
         {
            var code = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        testEnum.{|#0:SwitchPartially|}(item1: () => {}, item2: () => {});
                     }
                  }
               }
               """;

            var expectedCode = """

               using System;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestEnums;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                        testEnum.SwitchPartially(item1: static () => {}, item2: static () => {});
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments("SwitchPartially");
            await Verifier.VerifyCodeFixAsync(code, expectedCode, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }
      }
   }
}
