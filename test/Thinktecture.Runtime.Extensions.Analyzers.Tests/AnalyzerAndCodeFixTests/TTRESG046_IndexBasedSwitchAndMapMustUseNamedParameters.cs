using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG046_IndexBasedSwitchAndMapMustUseNamedParameters
{
   private const string _DIAGNOSTIC_ID = "TTRESG046";

   public class SmartEnum
   {
      public class SwitchWithAction
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testEnum.Switch(item1: static () => {},
                                           static () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testEnum.Switch(item1: static () => {},
                                           item2: static () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testEnum.SwitchPartially(@default: static item => {},
                                           item1: static () => {},
                                           static () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testEnum.SwitchPartially(static value => {}, // default
                                           item1: static () => {},
                                           item2: static () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testEnum.SwitchPartially(@default: static value => {},
                                           item1: static () => {},
                                           item2: static () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly]);
         }
      }

      public class SwitchWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testEnum.Switch(42,
                                           item1: static value => {},
                                           static value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testEnum.Switch(42,
                                           item1: static i => {},
                                           item2: static i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class SwitchPartiallyWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testEnum.SwitchPartially(42,
                                           @default: static (state, value) => {},
                                           item1: static value => {},
                                           static value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testEnum.SwitchPartially(42,
                                           static (state, value) => {}, // default
                                           item1: static value => {},
                                           item2: static value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testEnum.SwitchPartially(42,
                                           @default: static (value, item) => {},
                                           item1: static i => {},
                                           item2: static i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.Switch(item1: static () => 1,
                                           static () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.Switch(item1: static () => 1,
                                           item2: static () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class SwitchPartiallyWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(@default: static value => 0,
                                           item1: static () => 1,
                                           static () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(static value => 0, // default
                                           item1: static () => 1,
                                           item2: static () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(@default: static item => 0,
                                           item1: static () => 1,
                                           item2: static () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly]);
         }
      }

      public class SwitchWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.Switch(42,
                                           item1: static i => 1,
                                           static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.Switch(42,
                                           item1: static i => 1,
                                           item2: static i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }

      public class SwitchPartiallyWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: static (value, item) => 0,
                                           item1: static i => 1,
                                           static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           static (value, item) => 0, // default
                                           item1: static i => 1,
                                           item2: static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: static (value, item) => 0,
                                           item1: static i => 1,
                                           item2: static i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly]);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.Map(item1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.Map(item1: 1,
                                           item2: 2)|};
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
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testEnum.MapPartially(@default: 0,
                                           item1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testEnum.MapPartially(0, // default
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testEnum.MapPartially(
                                           @default: 0,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
         }
      }
   }

   public class AdHocUnion
   {
      public class SwitchWithAction
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.Switch(@string: static s => {},
                                           static i => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                                              {|#0:testUnion.Switch(@string: static s => {},
                                                                 int32: static i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.SwitchPartially(@default: static item => {},
                                           @string: static s => {},
                                           static i => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testUnion.SwitchPartially(static item => {},
                                           @string: static s => {},
                                           int32: static i => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.SwitchPartially(@default: static item => {},
                                           @string: static s => {},
                                           int32: static i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.Switch(42,
                                           @string: static (state, s) => {},
                                           static (state, i) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.Switch(42,
                                           @string: static (ctx, s) => {},
                                           int32: static (ctx, i) => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchPartiallyWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => {},
                                           @string: static (ctx, s) => {},
                                           static (ctx, i) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           static (ctx, value) => {}, // default
                                           @string: static (ctx, s) => {},
                                           int32: static (ctx, i) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => {},
                                           @string: static (ctx, s) => {},
                                           int32: static (ctx, i) => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Switch(@string: static s => 1,
                                           static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Switch(@string: static s => 1,
                                           int32: static i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchPartiallyWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(@default: static value => 0,
                                           @string: static s => 1,
                                           static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(static value => 0, // default
                                           @string: static s => 1,
                                           int32: static i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(@default: static item => 0,
                                           @string: static s => 1,
                                           int32: static i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Switch(42,
                                           @string: static (ctx, s) => 1,
                                           static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Switch(42,
                                           @string: static (ctx, s) => 1,
                                           int32: static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class SwitchPartiallyWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => 0,
                                           @string: static (ctx, s) => 1,
                                           static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           static (ctx, value) => 0, // default
                                           @string: static (ctx, s) => 1,
                                           int32: static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => 0,
                                           @string: static (ctx, s) => 1,
                                           int32: static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly]);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Map(@string: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Map(@string: 1,
                                           int32: 2)|};
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
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.MapPartially(@default: 0,
                                           @string: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.MapPartially(0, // default
                                           @string: 1,
                                           int32: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.MapPartially(
                                           @default: 0,
                                           @string: 1,
                                           int32: 2)|};
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
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.Switch(child1: static c => {},
                                           static c => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                                              {|#0:testUnion.Switch(child1: static c => {},
                                                                 child2: static c => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchPartiallyWithAction
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.SwitchPartially(@default: static item => {},
                                           child1: static c => {},
                                           static c => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testUnion.SwitchPartially(static d => {},
                                           child1: static c => {},
                                           child2: static c => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.SwitchPartially(@default: static d => {},
                                           child1: static c => {},
                                           child2: static c => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.Switch(42,
                                           child1: static (state, c) => {},
                                           static (state, c) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.Switch(42,
                                           child1: static (ctx, c) => {},
                                           child2: static (ctx, c) => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchPartiallyWithActionAndState
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => {},
                                           child1: static (ctx, c) => {},
                                           static (ctx, c) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           static (ctx, value) => {}, // default
                                           child1: static (ctx, s) => {},
                                           child2: static (ctx, i) => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => {},
                                           child1: static (ctx, c) => {},
                                           child2: static (ctx, c) => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Switch(child1: static c => 1,
                                           static c => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Switch(child1: static c => 1,
                                           child2: static c => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchPartiallyWithFunc
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(@default: static value => 0,
                                           child1: static c => 1,
                                           static c => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(static value => 0, // default
                                           child1: static c => 1,
                                           child2: static c => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(@default: static d => 0,
                                           child1: static c => 1,
                                           child2: static c => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_but_not_all_are_present()
         {
            var code = """

               using System;
               using System.Collections.Generic;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var value = new PlaceId.RegionId.InnerRegionId();
                        var res = value.SwitchPartially(
                           @default: static _ => "default",
                           regionId: static _ => "RegionId" ,
                           regionIdInnerRegionId: static _ => "InnerRegionId");
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_and_result_is_inlined_as_method_arg()
         {
            var code = """

               using System;
               using System.Collections.Generic;
               using Thinktecture;
               using Thinktecture.Runtime.Tests.TestRegularUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var value = new PlaceId.RegionId.InnerRegionId();
                        List<string> res = [];

                        res.Add(value.SwitchPartially(
                           @default: static _ => "default",
                           regionId: static _ => "RegionId" ,
                           regionIdInnerRegionId: static _ => "InnerRegionId"));
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Switch(42,
                                           child1: static (ctx, c) => 1,
                                           static (ctx, c) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Switch(42,
                                           child1: static (ctx, s) => 1,
                                           child2: static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class SwitchPartiallyWithFuncAndContext
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => 0,
                                           child1: static (ctx, c) => 1,
                                           static (ctx, c) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           static (ctx, value) => 0, // default
                                           child1: static (ctx, s) => 1,
                                           child2: static (ctx, i) => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: static (ctx, value) => 0,
                                           child1: static (ctx, c) => 1,
                                           child2: static (ctx, c) => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }

      public class Map
      {
         [Fact]
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.Map(child1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.Map(child1: 1,
                                           child2: 2)|};
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
         public async Task Should_trigger_without_named_arg()
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

                        var returnValue = {|#0:testUnion.MapPartially(@default: 0,
                                           child1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg()
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

                        var returnValue = {|#0:testUnion.MapPartially(0, // default
                                           child1: 1,
                                           child2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named()
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

                        var returnValue = {|#0:testUnion.MapPartially(
                                           @default: 0,
                                           child1: 1,
                                           child2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly]);
         }
      }
   }

   public class ComplexExpressions
   {
      [Fact]
      public async Task Should_trigger_when_lambda_is_unnamed_even_with_complex_body()
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

                     {|#0:testEnum.Switch(
                        item1: static () =>
                        {
                           Console.WriteLine("Item1");
                        },
                        static () =>
                        {
                           Console.WriteLine("Other");
                        })|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_all_named_with_trivia()
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

                     testEnum.Switch(
                        // First item
                        item1: static () => {},
                        // Second item
                        item2: static () => {});
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_with_only_first_parameter_unnamed()
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

                     {|#0:testEnum.Switch(
                        static () => {},
                        item2: static () => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_with_only_last_parameter_unnamed()
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

                     {|#0:testEnum.Switch(
                        item1: static () => {},
                        static () => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }
   }

   public class NestedCalls
   {
      [Fact]
      public async Task Should_trigger_on_outer_call_with_unnamed_parameters_even_when_inner_is_correct()
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
                     var testEnum1 = SmartEnum_StringBased.Item1;

                     {|#0:testEnum1.Switch(
                        item1: static () =>
                        {
                           SmartEnum_StringBased.Item2.Switch(item1: static () => {}, item2: static () => {});
                        },
                        static () => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_both_outer_and_inner_calls_with_unnamed_parameters()
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
                     var testEnum1 = SmartEnum_StringBased.Item1;

                     {|#0:testEnum1.Switch(
                        item1: static () =>
                        {
                           {|#1:SmartEnum_StringBased.Item2.Switch(item1: static () => {}, static () => {})|};
                        },
                        static () => {})|};
                  }
               }
            }
            """;

         var expected1 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         var expected2 = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(1).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected1, expected2);
      }

      [Fact]
      public async Task Should_not_trigger_when_both_outer_and_inner_have_all_named_parameters()
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
                     var testEnum1 = SmartEnum_StringBased.Item1;

                     testEnum1.Switch(
                        item1: static () =>
                        {
                           SmartEnum_StringBased.Item2.Switch(item1: static () => {}, item2: static () => {});
                        },
                        item2: static () => {});
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
      }
   }

   public class DefaultArguments
   {
      [Fact]
      public async Task Should_not_trigger_when_using_default_argument_values()
      {
         // This tests the case where arguments have default values
         // The analyzer should still require named parameters
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
      public async Task Should_trigger_on_partially_methods_even_with_all_parameters_provided()
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

                     {|#0:testEnum.SwitchPartially(
                        static value => {}, // default
                        item1: static () => {},
                        static () => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased_SwitchMapPartially));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly], expected);
      }
   }

   public class MethodReferences
   {
      [Fact]
      public async Task Should_trigger_with_method_reference_as_unnamed_parameter()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestEnums;

            namespace TestNamespace
            {
               public class Test
               {
                  private void Handler() { }

                  public void Do()
                  {
                     var testEnum = SmartEnum_StringBased.Item1;

                     {|#0:testEnum.Switch(
                        item1: Handler,
                        Handler)|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_with_all_method_references_named()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestEnums;

            namespace TestNamespace
            {
               public class Test
               {
                  private void Handler1() { }
                  private void Handler2() { }

                  public void Do()
                  {
                     var testEnum = SmartEnum_StringBased.Item1;

                     testEnum.Switch(
                        item1: Handler1,
                        item2: Handler2);
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
      }

      [Fact]
      public async Task Should_trigger_with_static_method_reference_as_unnamed_parameter()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestEnums;

            namespace TestNamespace
            {
               public class Test
               {
                  private static void Handler() { }

                  public void Do()
                  {
                     var testEnum = SmartEnum_StringBased.Item1;

                     {|#0:testEnum.Switch(
                        item1: Handler,
                        Handler)|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }
   }

   public class MixedUnionTypes
   {
      [Fact]
      public async Task Should_trigger_on_ad_hoc_union_with_mix_of_named_and_unnamed()
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

                     {|#0:testUnion.Switch(
                        @string: static s => {},
                        static i => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion_class_string_int));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute<,>).Assembly, typeof(TestUnion_class_string_int).Assembly], expected);
      }

      [Fact]
      public async Task Should_trigger_on_regular_union_with_mix_of_named_and_unnamed()
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

                     {|#0:testUnion.Switch(
                        child1: static c => {},
                        static c => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestUnion));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(UnionAttribute).Assembly, typeof(TestUnion).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_when_switch_call_is_chained()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestEnums;

            namespace TestNamespace
            {
               public class Test
               {
                  public int Do()
                  {
                     return SmartEnum_StringBased.Item1
                        .Switch(item1: static () => 1, item2: static () => 2);
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly]);
      }
   }

   public class ExtensionMethodScenarios
   {
      [Fact]
      public async Task Should_trigger_even_when_called_as_extension_method()
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

                     {|#0:testEnum.Switch(item1: static () => {}, static () => {})|};
                  }
               }
            }
            """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(SmartEnum_StringBased));
         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly], expected);
      }

      [Fact]
      public async Task Should_not_trigger_on_unrelated_extension_methods()
      {
         var code = """

            using System;
            using System.Linq;
            using Thinktecture;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var numbers = new[] { 1, 2, 3 };
                     var result = numbers.Select(static x => x * 2).Where(static x => x > 2);
                  }
               }
            }
            """;

         await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly]);
      }
   }
}
