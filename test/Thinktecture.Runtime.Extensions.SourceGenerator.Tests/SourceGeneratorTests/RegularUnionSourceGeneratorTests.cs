using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.RegularUnions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class RegularUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public RegularUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 20_000)
   {
   }

   [Fact]
   public async Task Should_generate_record_with_generic()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;

               public partial record Failure2(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_non_primary_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public sealed record Success(T Value) : Result<T>
               {
                  public Success(string value) : this(default(T)!)
                  {
                  }
               }

               public sealed record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_private_ctors_when_generating_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public sealed record Success(T Value) : Result<T>
               {
                  private Success(string value) : this(default(T)!)
                  {
                  }
               }

               public sealed record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_conversion_for_non_unique_generic_ctor_arguments()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Success2(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_generic_without_implicit_conversion()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(ConversionFromValue = ConversionOperatorsGeneration.None)]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_base_union_has_required_property()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public required string Property { get; set; }

               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_base_union_has_required_field()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public required string Field;

               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_union_has_required_property()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  public required string Property { get; set; }
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_has_required_field()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>
               {
                  public required string Field;
               }

               public partial record Failure(string Error) : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_has_an_interface()
   {
      var source = """
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1(IReadOnlyList<string> List) : TestUnion;

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_accepts_base_class_as_ctor_arg_1()
   {
      var source = """
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1(TestUnion Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_accepts_base_class_as_ctor_arg_2()
   {
      var source = """
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public class Child1 : TestUnion
               {
                  private Child1()
                  {
                  }

                  public sealed class Child2(TestUnion value) : Child1;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_accepts_base_class_as_ctor_arg_3()
   {
      var source = """
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public class Child1 : TestUnion
               {
                  private Child1()
                  {
                  }

                  public sealed class Child2(Child1 value) : Child1;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_implicit_conversion_if_derived_accepts_base_class_as_ctor_arg_4()
   {
      var source = """
         using System;
         using System.Collections.Generic;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class BaseClass;

            [Union]
            public partial class TestUnion : BaseClass
            {
               public class Child1(BaseClass value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_multiple_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>
               {
                  public Failure(int error) : this(error.ToString())
                  {
                  }
               };
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_multiple_explicit_conversions_if_configured()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(ConversionFromValue = ConversionOperatorsGeneration.Explicit)]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>
               {
                  public Failure(int error) : this(error.ToString())
                  {
                  }
               };
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_and_without_generic()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;

               public partial record Failure(string Error) : Result<T>;
            }

            [Union]
            public partial record Result
            {
               public partial record Success : Result;

               public partial record Failure(string Error) : Result;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs",
                        "Thinktecture.Tests.Result.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_record_with_non_default_ctor()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial record TestUnion
            {
               public string Name { get; }

               private TestUnion(string name)
               {
                  Name = name;
               }

               public sealed record Child1(string Name) : TestUnion(Name);

               public sealed record Child2(string Name) : TestUnion(Name);
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_without_ctor()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_class_with_classes_having_same_name()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;

               public sealed class Child2 : TestUnion
               {
                  public sealed class Child1 : TestUnion;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_special_chars()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class _1TestUnionWithSpecialChars
            {
               public sealed class _1Test : _1TestUnionWithSpecialChars;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests._1TestUnionWithSpecialChars.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_use_custom_SwitchMapStateParameterName()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(SwitchMapStateParameterName = "context")]
            public partial class TestUnion
            {
               public sealed class Test : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public void Should_not_generate_union_without_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      outputs.Should().BeEmpty();
   }

   [Fact]
   public async Task Should_handle_single_derived_type()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class OnlyChild : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_UnionSwitchMapOverload_attribute()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(Child1)])]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;
               public sealed class Child2 : TestUnion;
               public sealed class Child3 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_with_multiple_UnionSwitchMapOverload_attributes()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(Child1)])]
            [UnionSwitchMapOverload(StopAt = [typeof(Child1), typeof(Child2)])]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion;
               public sealed class Child2 : TestUnion;
               public sealed class Child3 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_protected_constructors_when_generating_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  protected Child1(string value)
                  {
                  }

                  public Child1(int value)
                  {
                  }
               }

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_internal_constructors_when_generating_implicit_conversions()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  internal Child1(string value)
                  {
                  }

                  public Child1(int value)
                  {
                  }
               }

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructors_with_zero_parameters()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1()
                  {
                  }
               }

               public sealed class Child2(string Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructors_with_two_parameters()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(string value1, string value2)
                  {
                  }
               }

               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructors_with_multiple_parameters()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(string a, int b, bool c)
                  {
                  }
               }

               public sealed class Child2(decimal Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_deep_inheritance_hierarchy_three_levels()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public class Level1 : TestUnion
               {
                  private Level1()
                  {
                  }

                  public class Level2 : Level1
                  {
                     private Level2()
                     {
                     }

                     public sealed class Level3(string Value) : Level2;
                  }
               }

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_multiple_branches_of_inheritance()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public class Branch1 : TestUnion
               {
                  private Branch1()
                  {
                  }

                  public sealed class Branch1Child1(int Value) : Branch1;
                  public sealed class Branch1Child2(string Value) : Branch1;
               }

               public class Branch2 : TestUnion
               {
                  private Branch2()
                  {
                  }

                  public sealed class Branch2Child1(bool Value) : Branch2;
                  public sealed class Branch2Child2(decimal Value) : Branch2;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_abstract_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public abstract class AbstractChild : TestUnion
               {
                  private AbstractChild()
                  {
                  }

                  public sealed class ConcreteGrandChild(string Value) : AbstractChild;
               }

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_mixed_sealed_and_non_sealed_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class SealedChild(string Value) : TestUnion;

               public class NonSealedChild : TestUnion
               {
                  private NonSealedChild()
                  {
                  }

                  public sealed class GrandChild(int Value) : NonSealedChild;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_constructor_with_optional_parameters()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(string value = "default")
                  {
                  }
               }

               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructor_with_ref_parameter()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(ref string value)
                  {
                  }
               }

               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructor_with_out_parameter()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(out int value)
                  {
                     value = 0;
                  }
               }

               public sealed class Child2(string Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_ignore_constructor_with_in_parameter()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(in int value)
                  {
                  }
               }

               public sealed class Child2(string Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_constructor_with_params_array()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(params string[] values)
                  {
                  }
               }

               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_switch_methods_when_disabled()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(SwitchMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestUnion
            {
               public sealed class Child1(string Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_map_methods_when_disabled()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(MapMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestUnion
            {
               public sealed class Child1(string Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_switch_and_map_methods_when_both_disabled()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(SwitchMethods = SwitchMapMethodsGeneration.None, MapMethods = SwitchMapMethodsGeneration.None)]
            public partial class TestUnion
            {
               public sealed class Child1(string Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_internal_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               internal sealed class InternalChild(string Value) : TestUnion;
               public sealed class PublicChild(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_union_inheriting_from_base_class()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class BaseClass
            {
               public string BaseProperty { get; set; } = "";
            }

            [Union]
            public partial class TestUnion : BaseClass
            {
               public sealed class Child1(string Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_multiple_constructor_overloads_with_mixed_accessibility()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1 : TestUnion
               {
                  public Child1(string value)
                  {
                  }

                  private Child1(int value)
                  {
                  }

                  protected Child1(bool value)
                  {
                  }

                  internal Child1(decimal value)
                  {
                  }
               }

               public sealed class Child2 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_derived_types_with_different_constructor_signatures()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1(string Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
               public sealed class Child3(bool Value) : TestUnion;
               public sealed class Child4(decimal Value) : TestUnion;
               public sealed class Child5(Guid Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_constructor_with_nullable_reference_type_parameter()
   {
      var source = """
         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1(string? Value) : TestUnion;
               public sealed class Child2(int Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_constructor_with_nullable_value_type_parameter()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public sealed class Child1(int? Value) : TestUnion;
               public sealed class Child2(string Value) : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_very_deep_inheritance_hierarchy_five_levels()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            public partial class TestUnion
            {
               public class L1 : TestUnion
               {
                  private L1() { }

                  public class L2 : L1
                  {
                     private L2() { }

                     public class L3 : L2
                     {
                        private L3() { }

                        public class L4 : L3
                        {
                           private L4() { }

                           public sealed class L5(string Value) : L4;
                        }
                     }
                  }
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }
}
