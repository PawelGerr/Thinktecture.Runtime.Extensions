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
   private static readonly Assembly[] _smartEnumCustomStateReferences = [typeof(ComplexValueObjectAttribute).Assembly, typeof(SmartEnum_CustomSwitchMapStateParameterName).Assembly];
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

                     var result = testEnum.[||]{|CS7036:MapPartially<string>|}();
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

                     testEnum.[||]{|CS1501:Switch|}();
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

                     var result = testEnum.[||]{|CS1501:Switch<string>|}();
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

                     var result = testEnum.[||]{|CS7036:Map<string>|}();
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

                     testUnion.[||]{|CS1501:Switch|}();
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

                     var result = testUnion.[||]{|CS1501:Switch<int>|}();
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

                     testUnion.[||]{|CS1501:Switch|}();
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

                     var result = testUnion.[||]{|CS1501:Switch<string>|}();
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

                     var result = testUnion.[||]{|CS7036:Map<string>|}();
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

                     var result = testUnion.[||]{|CS7036:Map<string>|}();
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
                     foo.[||]{|CS7036:Switch|}();
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

                     testEnum.[||]{|CS1501:Switch<string>|}();
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
                        item1: static state1 => { },
                        item2: static state1 => { });
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

                     var result = testEnum.[||]{|CS7036:Switch<string, string>|}();
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
                        item1: static state1 => throw new System.NotImplementedException(),
                        item2: static state1 => throw new System.NotImplementedException());
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

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                        @string: static (state1, x) => { },
                        int32: static (state1, x) => { });
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

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                        child1: static (state1, x) => { },
                        child2: static (state1, x) => { });
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

                     testEnum.[||]{|CS1501:Switch|}(item1: static () => { });
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

                     testEnum.[||]{|CS1501:Switch|}(() => { });
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

                     var result = testEnum.[||]{|CS7036:Switch<string, string>|}(state, x => throw new System.NotImplementedException());
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
                        item2: static state1 => throw new System.NotImplementedException());
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

                     testUnion.[||]{|CS1501:Switch|}(@string: static x => { });
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

                     testUnion.[||]{|CS1501:Switch|}(x => { });
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

                     testUnion.[||]{|CS1501:Switch|}(child1: static x => { });
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

                     testUnion.[||]{|CS1501:Switch|}(x => { });
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

                     var result = testEnum.[||]{|CS7036:Map<string>|}(item1: "hello");
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

                     var result = testUnion.[||]{|CS7036:Map<string>|}(@string: "hello");
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

                     var result = testUnion.[||]{|CS7036:Map<string>|}(child1: "hello");
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

                     testEnum.[||]{|CS7036:Switch<string>|}(state: state, item1: static x => { });
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
                        item2: static state1 => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Action_WithState_CustomStateParameterName
   {
      [Fact]
      public async Task Should_generate_state_action_arguments_with_custom_state_parameter_name()
      {
         // A local named "context" is in scope at the invocation, and the union's state parameter is
         // also named "context". The generated state lambda parameter must not reuse "context",
         // because a lambda parameter cannot shadow an enclosing local (CS0136). The refactoring
         // therefore renames the state lambda parameter to "context1" while the state argument still
         // references the enclosing local "context".
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     testUnion.Switch<string>(
                        context: context,
                        @string: static (context1, x) => { },
                        int32: static (context1, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Action_WithState_KeywordStateParameterName
   {
      [Fact]
      public async Task Should_escape_keyword_state_parameter_name_in_generated_lambda()
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
                     var testUnion = (TestUnionWithKeywordSwitchMapStateParameterName)"hello";
                     string @default = "";

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                     var testUnion = (TestUnionWithKeywordSwitchMapStateParameterName)"hello";
                     string @default = "";

                     testUnion.Switch<string>(
                        @default: @default,
                        @string: static (default1, x) => { },
                        int32: static (default1, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Action_WithState_StateParameterNameEqualsValueParameterName
   {
      [Fact]
      public async Task Should_not_produce_duplicate_lambda_parameter_when_state_name_equals_value_name()
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
                     var testUnion = (TestUnionWithXSwitchMapStateParameterName)"hello";
                     string x = "";

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                     var testUnion = (TestUnionWithXSwitchMapStateParameterName)"hello";
                     string x = "";

                     testUnion.Switch<string>(
                        x: x,
                        @string: static (x1, value) => { },
                        int32: static (x1, value) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Action_ValueParameterNameCollidesWithInScopeLocal
   {
      [Fact]
      public async Task Should_avoid_value_parameter_name_that_collides_with_in_scope_local()
      {
         // A local named "x" is in scope at the invocation. The generated value lambda parameter must
         // not reuse "x", because a lambda parameter cannot shadow an enclosing local (CS0136). The
         // refactoring therefore falls back to the next free candidate name "value".
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
                     int x = 5;

                     testUnion.[||]{|CS1501:Switch|}();
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
                     int x = 5;

                     testUnion.Switch(
                        @string: static value => { },
                        int32: static value => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_Func_WithState_CustomStateParameterName
   {
      [Fact]
      public async Task Should_generate_state_func_arguments_with_custom_state_parameter_name()
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     var result = testUnion.[||]{|CS7036:Switch<string, string>|}();
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     var result = testUnion.Switch<string, string>(
                        context: context,
                        @string: static (context1, x) => throw new System.NotImplementedException(),
                        int32: static (context1, x) => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences);
      }
   }

   public class RegularUnion_Switch_Action_WithState_CustomStateParameterName
   {
      [Fact]
      public async Task Should_generate_state_action_arguments_with_custom_state_parameter_name()
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
                     TestUnionWithCustomSwitchMapStateParameterName testUnion = new TestUnionWithCustomSwitchMapStateParameterName.Child1("test");
                     string context = "";

                     testUnion.[||]{|CS1501:Switch<string>|}();
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
                     TestUnionWithCustomSwitchMapStateParameterName testUnion = new TestUnionWithCustomSwitchMapStateParameterName.Child1("test");
                     string context = "";

                     testUnion.Switch<string>(
                        context: context,
                        child1: static (context1, x) => { },
                        child2: static (context1, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _regularUnionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_SwitchPartially_Action_WithState_CustomStateParameterName
   {
      [Fact]
      public async Task Should_generate_state_action_arguments_with_custom_state_parameter_name()
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     testUnion.[||]{|CS1501:SwitchPartially<string>|}();
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     testUnion.SwitchPartially<string>(
                        context: context,
                        @default: static (context1, x) => { },
                        @string: static (context1, x) => { },
                        int32: static (context1, x) => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_SwitchPartially_Func_WithState_CustomStateParameterName
   {
      [Fact]
      public async Task Should_generate_state_func_arguments_with_custom_state_parameter_name()
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     var result = testUnion.[||]{|CS7036:SwitchPartially<string, string>|}();
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
                     var testUnion = (TestUnionWithCustomSwitchMapStateParameterName)"hello";
                     string context = "";

                     var result = testUnion.SwitchPartially<string, string>(
                        context: context,
                        @default: static (context1, x) => throw new System.NotImplementedException(),
                        @string: static (context1, x) => throw new System.NotImplementedException(),
                        int32: static (context1, x) => throw new System.NotImplementedException());
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences);
      }
   }

   public class CursorOnForeignNestedInvocation
   {
      [Fact]
      public async Task Should_offer_refactoring_when_cursor_is_on_foreign_nested_invocation_with_matching_name()
      {
         var code = """

            using System;
            using Thinktecture;
            using Thinktecture.Runtime.Tests.TestEnums;

            namespace TestNamespace
            {
               public static class Ext
               {
                  public static int Map(this int value, Func<int, int> selector) => selector(value);
               }

               public class Test
               {
                  public void Do()
                  {
                     var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                     testEnum.SwitchPartially(
                        @default: static x => { },
                        item1: static () => { var value = 5.[||]Map(v => v + 1); });
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
               public static class Ext
               {
                  public static int Map(this int value, Func<int, int> selector) => selector(value);
               }

               public class Test
               {
                  public void Do()
                  {
                     var testEnum = SmartEnum_StringBased_SwitchMapPartially.Item1;

                     testEnum.SwitchPartially(
                        @default: static x => { },
                        item1: static () => { var value = 5.Map(v => v + 1); },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_NonTrailingNamedArgument
   {
      [Fact]
      public async Task Should_not_duplicate_positional_argument_when_named_argument_is_non_trailing()
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

                     testEnum.[||]SwitchPartially(@default: static x => { }, static () => { });
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
                        static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_PreservesComments
   {
      [Fact]
      public async Task Should_preserve_comment_on_existing_argument()
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

                     testEnum.[||]SwitchPartially(
                        // must run first
                        @default: static x => { });
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
                        // must run first
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

   public class SmartEnum_SwitchPartially_PreservesCommentBehindSeparatorComma
   {
      [Fact]
      public async Task Should_preserve_comment_that_follows_the_comma_of_an_existing_argument()
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

                     testEnum.[||]SwitchPartially(
                        @default: static x => { }, // must run first
                        item1: static () => { });
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
                        @default: static x => { }, // must run first
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_PreservesCommentOnClosingParen
   {
      [Fact]
      public async Task Should_preserve_comment_attached_to_closing_paren()
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

                     testEnum.[||]SwitchPartially(
                        @default: static x => { }
                        // keep this note
                        );
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
                        item2: static () => { }
                        // keep this note
                        );
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_PreservesPreprocessorDirectives
   {
      [Fact]
      public async Task Should_preserve_preprocessor_directives_in_argument_and_closing_paren_trivia()
      {
         // The raw strings below are not indented, because the formatter places preprocessor directives
         // in column 0, and a raw string literal cannot contain a line with less indentation than its
         // closing delimiter.
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

         testEnum.[||]SwitchPartially(
#if true
            @default: static x => { }
#endif
            );
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
#if true
            @default: static x => { },
            item1: static () => { },
            item2: static () => { }
#endif
            );
      }
   }
}
""";

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_WithState
   {
      [Fact]
      public async Task Should_use_state_parameter_name_for_single_parameter_lambdas()
      {
         // In a state overload every delegate receives the state first. The items of this Smart Enum
         // carry no value, so their delegates have exactly one parameter, and that parameter is the
         // state. It therefore has to use the same (collision-renamed) name as the first parameter of
         // the two-parameter "@default" delegate.
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
                     string state = "";

                     testEnum.[||]{|CS1501:SwitchPartially<string>|}();
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
                     string state = "";

                     testEnum.SwitchPartially<string>(
                        state: state,
                        @default: static (state1, x) => { },
                        item1: static state1 => { },
                        item2: static state1 => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_Switch_WithState_RepeatedStateNameCollision
   {
      [Fact]
      public async Task Should_append_next_free_number_when_state_name_and_first_numbered_name_are_taken()
      {
         // The configured state parameter name "context" and the first numbered fallback "context1"
         // are both taken by an enclosing local, so the generated lambda parameter must skip to
         // "context2" to avoid shadowing (CS0136).
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
                     var testEnum = SmartEnum_CustomSwitchMapStateParameterName.Item1;
                     string context = "";
                     string context1 = "";

                     testEnum.[||]{|CS1501:Switch<string>|}();
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
                     var testEnum = SmartEnum_CustomSwitchMapStateParameterName.Item1;
                     string context = "";
                     string context1 = "";

                     testEnum.Switch<string>(
                        context: context,
                        item1: static context2 => { },
                        item2: static context2 => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _smartEnumCustomStateReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_ValueParameterFallback
   {
      [Fact]
      public async Task Should_fall_back_to_numbered_value_name_when_all_candidates_are_taken()
      {
         // Every candidate name ("x", "value", "v", "arg", "item") and the first numbered fallback
         // ("value1") are taken by an enclosing local, so the generated lambda parameter is "value2".
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
                     int x = 0;
                     int value = 0;
                     int v = 0;
                     int arg = 0;
                     int item = 0;
                     int value1 = 0;

                     testUnion.[||]{|CS1501:Switch|}();
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
                     int x = 0;
                     int value = 0;
                     int v = 0;
                     int arg = 0;
                     int item = 0;
                     int value1 = 0;

                     testUnion.Switch(
                        @string: static value2 => { },
                        int32: static value2 => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class AdHocUnion_Switch_ReservedLocalDeclaredAfterInvocation
   {
      [Fact]
      public async Task Should_treat_local_declared_after_invocation_as_reserved()
      {
         // The local "x" is declared after the invocation, but its scope is the whole block. A lambda
         // parameter named "x" would therefore still be a forbidden shadowing (CS0136), so the
         // refactoring has to fall back to the next free candidate name "value".
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

                     testUnion.[||]{|CS1501:Switch|}();

                     int x = 5;
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
                        @string: static value => { },
                        int32: static value => { });

                     int x = 5;
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _unionReferences, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_PreservesCommentOnNonFirstArgument
   {
      [Fact]
      public async Task Should_preserve_comment_on_non_first_argument()
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

                     testEnum.[||]SwitchPartially(
                        @default: static x => { },
                        // handled explicitly
                        item1: static () => { });
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
                        // handled explicitly
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }

   public class SmartEnum_SwitchPartially_TrailingCommentBeforeGeneratedComma
   {
      [Fact]
      public async Task Should_keep_comma_on_argument_line_when_existing_argument_has_trailing_comment()
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

                     testEnum.[||]SwitchPartially(
                        @default: static x => { } // must run first
                        );
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
                        @default: static x => { }, // must run first
                        item1: static () => { },
                        item2: static () => { });
                  }
               }
            }
            """;

         await Verifier.VerifyRefactoringAsync(code, fixedCode, _references, codeActionIndex: 0);
      }
   }
}
