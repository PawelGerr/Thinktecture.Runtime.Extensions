using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestEnums;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeFixVerifier<Thinktecture.CodeAnalysis.Diagnostics.ThinktectureRuntimeExtensionsAnalyzer, Thinktecture.CodeAnalysis.CodeFixes.ThinktectureRuntimeExtensionsCodeFixProvider>;

namespace Thinktecture.Runtime.Tests.AnalyzerAndCodeFixTests;

// ReSharper disable InconsistentNaming
public class TTRESG046_IndexBasedSwitchAndMapMustUseNamedParameters
{
   private const string _DIAGNOSTIC_ID = "TTRESG046";

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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
      }
   }

   public class SwitchWithActionAndContext
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
      }
   }

   public class SwitchPartiallyWithActionAndContext
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
                                                @default: (ctx, value) => {},
                                                item1: value => {},
                                                value => {})|};
                          }
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
                                                (ctx, value) => {}, // default
                                                item1: value => {},
                                                item2: value => {})|};
                          }
                       }
                    }
                    """;

         var expected = Verifier.Diagnostic(_DIAGNOSTIC_ID).WithLocation(0).WithArguments(nameof(ValidTestEnum));
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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
         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(TestEnum).Assembly }, expected);
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
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

         await Verifier.VerifyAnalyzerAsync(code, new[] { typeof(ComplexValueObjectAttribute).Assembly, typeof(ValidTestEnum).Assembly });
      }
   }
}
