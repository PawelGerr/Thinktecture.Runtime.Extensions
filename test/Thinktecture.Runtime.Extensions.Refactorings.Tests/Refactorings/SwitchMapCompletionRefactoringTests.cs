using System.Reflection;
using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestAdHocUnions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Verifier = Thinktecture.Runtime.Tests.Verifiers.CodeRefactoringVerifier<Thinktecture.CodeAnalysis.Refactorings.SwitchMapCompletionRefactoringProvider>;

namespace Thinktecture.Runtime.Tests.Refactorings;

// ReSharper disable InconsistentNaming
public class SwitchMapCompletionRefactoringTests
{
   private static readonly Assembly[] _references = [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased_SwitchMapPartially).Assembly];
   private static readonly Assembly[] _smartEnumReferences = [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_StringBased).Assembly];
   private static readonly Assembly[] _unionReferences = [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestUnion_class_string_int).Assembly];
   private static readonly Assembly[] _regularUnionReferences = [typeof(ComplexValueObjectAttribute).Assembly, typeof(TestUnion).Assembly];

   public class SmartEnum_SwitchPartially
   {
      [Fact]
      public async Task Should_generate_switch_partially_arguments()
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

                     testEnum.[||]SwitchPartially();
                  }
               }
            }
            """;

         var fixedCode = """

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

                     testEnum.SwitchPartially(
                        @default: static x => { },
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_MapPartially
   {
      [Fact]
      public async Task Should_generate_map_partially_arguments()
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

                     var result = testEnum.[||]MapPartially<string>();
                  }
               }
            }
            """;

         var fixedCode = """

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

                     var result = testEnum.MapPartially<string>(
                        @default: default,
                        item1: default,
                        item2: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references);
      }
   }

   public class AdHocUnion_SwitchPartially
   {
      [Fact]
      public async Task Should_generate_switch_partially_arguments()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.[||]SwitchPartially();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.SwitchPartially(
                        @default: static x => { },
                        @string: static x => { },
                        int32: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class NoRefactoring_WhenAllArgsPresent
   {
      [Fact]
      public async Task Should_not_offer_refactoring_when_all_args_present()
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

                     testEnum.[||]SwitchPartially(@default: static x => { }, item1: static () => { }, item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyNoRefactoringAsync(code, _references);
      }
   }

   public class CursorOnNestedInvocation
   {
      [Fact]
      public async Task Should_offer_refactoring_when_cursor_is_on_nested_invocation_inside_switch()
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

                     testEnum.SwitchPartially(
                        @default: static x => { },
                        item1: static () => { [||]Console.WriteLine(); });
                  }
               }
            }
            """;

         var fixedCode = """

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

                     testEnum.SwitchPartially(
                        @default: static x => { },
                        item1: static () => { Console.WriteLine(); },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_Action
   {
      [Fact]
      public async Task Should_generate_action_arguments()
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

                     testEnum.[||]Switch();
                  }
               }
            }
            """;

         var fixedCode = """

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
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_Func
   {
      [Fact]
      public async Task Should_generate_func_arguments()
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

                     var result = testEnum.[||]Switch<string>();
                  }
               }
            }
            """;

         var fixedCode = """

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

                     var result = testEnum.Switch<string>(
                        item1: static () => throw new System.NotImplementedException(),
                        item2: static () => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 1);
      }
   }

   public class SmartEnum_Map
   {
      [Fact]
      public async Task Should_generate_map_arguments()
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

                     var result = testEnum.[||]Map<string>();
                  }
               }
            }
            """;

         var fixedCode = """

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

                     var result = testEnum.Map<string>(
                        item1: default,
                        item2: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences);
      }
   }

   public class AdHocUnion_Switch_Action
   {
      [Fact]
      public async Task Should_generate_action_arguments_with_typed_lambdas()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.[||]Switch();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.Switch(
                        @string: static x => { },
                        int32: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Func
   {
      [Fact]
      public async Task Should_generate_func_arguments_with_typed_lambdas()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.[||]Switch<int>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.Switch<int>(
                        @string: static x => throw new System.NotImplementedException(),
                        int32: static x => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 1);
      }
   }

   public class RegularUnion_Switch_Action
   {
      [Fact]
      public async Task Should_generate_action_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.[||]Switch();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.Switch(
                        child1: static x => { },
                        child2: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 0);
      }
   }

   public class RegularUnion_Switch_Func
   {
      [Fact]
      public async Task Should_generate_func_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.[||]Switch<string>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.Switch<string>(
                        child1: static x => throw new System.NotImplementedException(),
                        child2: static x => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 1);
      }
   }

   public class AdHocUnion_Map
   {
      [Fact]
      public async Task Should_generate_map_arguments()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.[||]Map<string>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.Map<string>(
                        @string: default,
                        int32: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences);
      }
   }

   public class RegularUnion_Map
   {
      [Fact]
      public async Task Should_generate_map_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.[||]Map<string>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.Map<string>(
                        child1: default,
                        child2: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences);
      }
   }

   public class NoRefactoring_ForNonThinktectureTypes
   {
      [Fact]
      public async Task Should_not_offer_refactoring_for_non_thinktecture_types()
      {
         var code = """

            using System;

            namespace TestNamespace
            {
               public class Foo
               {
                  public void Switch(int x, int y) { }
               }

               public class Test
               {
                  public void Do()
                  {
                     var foo = new Foo();
                     foo.[||]Switch();
                  }
               }
            }
            """;

         await Verifier.VerifyNoRefactoringAsync(code, []);
      }
   }

   public class SmartEnum_Switch_Action_WithState
   {
      [Fact]
      public async Task Should_generate_state_action_arguments()
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
                     string state = "";

                     testEnum.[||]Switch<string>();
                  }
               }
            }
            """;

         var fixedCode = """

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
                     string state = "";

                     testEnum.Switch<string>(
                        state: state,
                        item1: static x => { },
                        item2: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_Func_WithState
   {
      [Fact]
      public async Task Should_generate_state_func_arguments()
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
                     string state = "";

                     var result = testEnum.[||]Switch<string, string>();
                  }
               }
            }
            """;

         var fixedCode = """

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
                     string state = "";

                     var result = testEnum.Switch<string, string>(
                        state: state,
                        item1: static x => throw new System.NotImplementedException(),
                        item2: static x => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences);
      }
   }

   public class AdHocUnion_Switch_Action_WithState
   {
      [Fact]
      public async Task Should_generate_state_action_arguments()
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
                     var testUnion = (TestUnion_class_string_int)"hello";
                     string state = "";

                     testUnion.[||]Switch<string>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";
                     string state = "";

                     testUnion.Switch<string>(
                        state: state,
                        @string: static (state, x) => { },
                        int32: static (state, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class RegularUnion_Switch_Action_WithState
   {
      [Fact]
      public async Task Should_generate_state_action_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");
                     string state = "";

                     testUnion.[||]Switch<string>();
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");
                     string state = "";

                     testUnion.Switch<string>(
                        state: state,
                        child1: static (state, x) => { },
                        child2: static (state, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments_when_some_already_provided()
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

                     testEnum.[||]Switch(item1: static () => { });
                  }
               }
            }
            """;

         var fixedCode = """

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
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_PartialArgs_Positional
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments_when_positional_arg_provided()
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

                     testEnum.[||]Switch(() => { });
                  }
               }
            }
            """;

         var fixedCode = """

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
                        () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_Func_WithState_PartialArgs_Positional
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments_when_positional_args_provided()
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
                     string state = "";

                     var result = testEnum.[||]Switch<string, string>(state, x => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         var fixedCode = """

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
                     string state = "";

                     var result = testEnum.Switch<string, string>(
                        state,
                        x => throw new System.NotImplementedException(),
                        item2: static x => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences);
      }
   }

   public class AdHocUnion_Switch_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.[||]Switch(@string: static x => { });
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.Switch(
                        @string: static x => { },
                        int32: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_PartialArgs_Positional
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments_when_positional_arg_provided()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.[||]Switch(x => { });
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     testUnion.Switch(
                        x => { },
                        int32: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class RegularUnion_Switch_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.[||]Switch(child1: static x => { });
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.Switch(
                        child1: static x => { },
                        child2: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 0);
      }
   }

   public class RegularUnion_Switch_PartialArgs_Positional
   {
      [Fact]
      public async Task Should_generate_only_missing_arguments_when_positional_arg_provided()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.[||]Switch(x => { });
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     testUnion.Switch(
                        x => { },
                        child2: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Map_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_map_arguments()
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

                     var result = testEnum.[||]Map<string>(item1: "hello");
                  }
               }
            }
            """;

         var fixedCode = """

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

                     var result = testEnum.Map<string>(
                        item1: "hello",
                        item2: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences);
      }
   }

   public class AdHocUnion_Map_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_map_arguments()
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
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.[||]Map<string>(@string: "hello");
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestAdHocUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     var testUnion = (TestUnion_class_string_int)"hello";

                     var result = testUnion.Map<string>(
                        @string: "hello",
                        int32: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences);
      }
   }

   public class RegularUnion_Map_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_map_arguments()
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
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.[||]Map<string>(child1: "hello");
                  }
               }
            }
            """;

         var fixedCode = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestRegularUnions;

            namespace TestNamespace
            {
               public class Test
               {
                  public void Do()
                  {
                     TestUnion testUnion = new TestUnion.Child1("test");

                     var result = testUnion.Map<string>(
                        child1: "hello",
                        child2: default);
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences);
      }
   }

   public class SmartEnum_Switch_Action_WithState_PartialArgs
   {
      [Fact]
      public async Task Should_generate_only_missing_state_action_arguments()
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
                     string state = "";

                     testEnum.[||]Switch<string>(state: state, item1: static x => { });
                  }
               }
            }
            """;

         var fixedCode = """

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
                     string state = "";

                     testEnum.Switch<string>(
                        state: state,
                        item1: static x => { },
                        item2: static x => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }
}
