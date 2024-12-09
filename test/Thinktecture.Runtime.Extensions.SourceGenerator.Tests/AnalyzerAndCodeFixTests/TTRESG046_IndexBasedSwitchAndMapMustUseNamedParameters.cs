using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestUnions;
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.Switch(item1: () => {},
                                           () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(item => {}, // invalid item callback
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(invalid: item => {},
                                           () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.Switch(item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(invalid: item => {},
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(@default: item => {},
                                           item1: () => {},
                                           () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(value => {}, // default
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(item => {}, // invalid item callback
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(@default: value => {},
                                           invalid: item => {},
                                           () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(value => {}, // default
                                           invalid: item => {},
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(@default: value => {},
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(@default: value => {},
                                           invalid: item => {},
                                           item1: () => {},
                                           item2: () => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.Switch(42,
                                           item1: value => {},
                                           value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(42,
                                           (value, item) => {}, // invalid item callback
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(42,
                                           invalid: (value, item) => {},
                                           value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.Switch(42,
                                           item1: i => {},
                                           item2: i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.Switch(42,
                                           invalid: (value, item) => {},
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           @default: (state, value) => {},
                                           item1: value => {},
                                           value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           (state, value) => {}, // default
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           (value, item) => {}, // invalid item callback
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => {},
                                           invalid: (value, item) => {},
                                           value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           (value, item) => {}, // default
                                           invalid: (value, item) => {},
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => {},
                                           item1: i => {},
                                           item2: i => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => {},
                                           invalid: (value, item) => {},
                                           item1: value => {},
                                           item2: value => {})|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(item1: () => 1,
                                           () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(item => 0,
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(invalid: item => 0,
                                           () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(invalid: item => 0,
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(@default: value => 0,
                                           item1: () => 1,
                                           () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(value => 0, // default
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(@default: item => 0,
                                           item => -1, // invalid
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(@default: item => 0,
                                           invalid: item => -1,
                                           () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(item => 0,
                                           invalid: item => -1,
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(@default: item => 0,
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(@default: item => 0,
                                           invalid: item => -1,
                                           item1: () => 1,
                                           item2: () => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(42,
                                           item1: i => 1,
                                           i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(42,
                                           (value, item) => 0,
                                           item1: value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(42,
                                           invalid: (value, item) => 0,
                                           value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(42,
                                           item1: i => 1,
                                           item2: i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Switch(42,
                                           invalid: (value, item) => 0,
                                           item1: value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => 0,
                                           item1: i => 1,
                                           i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           (value, item) => 0, // default
                                           item1: i => 1,
                                           item2: i => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           (value, item) => 0,
                                           item1: value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => 0,
                                           invalid: (value, item) => -1,
                                           value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           (value, item) => 0, // default
                                           invalid: (value, item) => -1,
                                           item1: value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => 0,
                                           item1: i => 1,
                                           item2: i => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.SwitchPartially(42,
                                           @default: (value, item) => 0,
                                           invalid: (value, item) => -1,
                                           item1: value => 1,
                                           item2: value => 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Map(item1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Map(0,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Map(invalid: 0,
                                           1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Map(item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.Map(invalid: 0,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(@default: 0,
                                           item1: 1,
                                           2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(0, // default
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_invalid_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(@default: 0,
                                           -1, // invalid
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_default_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(0, // default
                                           invalid: -1,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
         }

         [Fact]
         public async Task Should_trigger_without_named_arg_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(@default: 0,
                                           invalid: -1,
                                           1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(TestEnum));
            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly], expected);
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
                        var testEnum = ValidTestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(
                                           @default: 0,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
         }

         [Fact]
         public async Task Should_not_trigger_when_all_args_are_named_having_validatable_enum()
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
                        var testEnum = TestEnum.Item1;
               
                        var returnValue = {|#0:testEnum.MapPartially(
                                           @default: 0,
                                           invalid: -1,
                                           item1: 1,
                                           item2: 2)|};
                     }
                  }
               }
               """;

            await Verifier.VerifyAnalyzerAsync(code, [typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly]);
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
               
                        {|#0:testUnion.Switch(@string: s => {},
                                           i => {})|};
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
               
                                              {|#0:testUnion.Switch(@string: s => {},
                                                                 int32: i => {})|};
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
               
                        {|#0:testUnion.SwitchPartially(@default: item => {},
                                           @string: s => {},
                                           i => {})|};
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
               
                        {|#0:testUnion.SwitchPartially(item => {},
                                           @string: s => {},
                                           int32: i => {})|};
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
               
                        {|#0:testUnion.SwitchPartially(@default: item => {},
                                           @string: s => {},
                                           int32: i => {})|};
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
                                           @string: (state, s) => {},
                                           (state, i) => {})|};
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
                                           @string: (ctx, s) => {},
                                           int32: (ctx, i) => {})|};
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
                                           @default: (ctx, value) => {},
                                           @string: (ctx, s) => {},
                                           (ctx, i) => {})|};
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
                                           (ctx, value) => {}, // default
                                           @string: (ctx, s) => {},
                                           int32: (ctx, i) => {})|};
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
                                           @default: (ctx, value) => {},
                                           @string: (ctx, s) => {},
                                           int32: (ctx, i) => {})|};
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
               
                        var returnValue = {|#0:testUnion.Switch(@string: s => 1,
                                           i => 2)|};
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
               
                        var returnValue = {|#0:testUnion.Switch(@string: s => 1,
                                           int32: i => 2)|};
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
               
                        var returnValue = {|#0:testUnion.SwitchPartially(@default: value => 0,
                                           @string: s => 1,
                                           i => 2)|};
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
               
                        var returnValue = {|#0:testUnion.SwitchPartially(value => 0, // default
                                           @string: s => 1,
                                           int32: i => 2)|};
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
               
                        var returnValue = {|#0:testUnion.SwitchPartially(@default: item => 0,
                                           @string: s => 1,
                                           int32: i => 2)|};
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
                                           @string: (ctx, s) => 1,
                                           (ctx, i) => 2)|};
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
                                           @string: (ctx, s) => 1,
                                           int32: (ctx, i) => 2)|};
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
                                           @default: (ctx, value) => 0,
                                           @string: (ctx, s) => 1,
                                           (ctx, i) => 2)|};
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
                                           (ctx, value) => 0, // default
                                           @string: (ctx, s) => 1,
                                           int32: (ctx, i) => 2)|};
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
                                           @default: (ctx, value) => 0,
                                           @string: (ctx, s) => 1,
                                           int32: (ctx, i) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.Switch(child1: c => {},
                                           c => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                                              {|#0:testUnion.Switch(child1: c => {},
                                                                 child2: c => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                         var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(@default: item => {},
                                           child1: c => {},
                                           c => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                         var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(d => {},
                                           child1: c => {},
                                           child2: c => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(@default: d => {},
                                           child1: c => {},
                                           child2: c => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.Switch(42,
                                           child1: (state, c) => {},
                                           (state, c) => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.Switch(42,
                                           child1: (ctx, c) => {},
                                           child2: (ctx, c) => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(42,
                                           @default: (ctx, value) => {},
                                           child1: (ctx, c) => {},
                                           (ctx, c) => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(42,
                                           (ctx, value) => {}, // default
                                           child1: (ctx, s) => {},
                                           child2: (ctx, i) => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        {|#0:testUnion.SwitchPartially(42,
                                           @default: (ctx, value) => {},
                                           child1: (ctx, c) => {},
                                           child2: (ctx, c) => {})|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.Switch(child1: c => 1,
                                           c => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.Switch(child1: c => 1,
                                           child2: c => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(@default: value => 0,
                                           child1: c => 1,
                                           c => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(value => 0, // default
                                           child1: c => 1,
                                           child2: c => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(@default: d => 0,
                                           child1: c => 1,
                                           child2: c => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.Switch(42,
                                           child1: (ctx, c) => 1,
                                           (ctx, c) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.Switch(42,
                                           child1: (ctx, s) => 1,
                                           child2: (ctx, i) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: (ctx, value) => 0,
                                           child1: (ctx, c) => 1,
                                           (ctx, c) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           (ctx, value) => 0, // default
                                           child1: (ctx, s) => 1,
                                           child2: (ctx, i) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

               namespace TestNamespace
               {
                  public class Test
                  {
                     public void Do()
                     {
                        var testUnion = new TestUnion.Child1("text");
               
                        var returnValue = {|#0:testUnion.SwitchPartially(42,
                                           @default: (ctx, value) => 0,
                                           child1: (ctx, c) => 1,
                                           child2: (ctx, c) => 2)|};
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
               using Thinktecture.Runtime.Tests.TestUnions;

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
               using Thinktecture.Runtime.Tests.TestUnions;

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
               using Thinktecture.Runtime.Tests.TestUnions;

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
               using Thinktecture.Runtime.Tests.TestUnions;

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
               using Thinktecture.Runtime.Tests.TestUnions;

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
}
