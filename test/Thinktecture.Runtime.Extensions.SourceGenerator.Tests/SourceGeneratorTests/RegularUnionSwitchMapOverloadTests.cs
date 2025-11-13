using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.RegularUnions;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class RegularUnionSwitchMapOverloadTests : SourceGeneratorTestsBase
{
   public RegularUnionSwitchMapOverloadTests(ITestOutputHelper output)
      : base(output, 40_000)
   {
   }

   [Fact]
   public async Task Should_not_generate_switch_map_overload_when_type_has_no_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(ConcreteType1)])]
            public partial class TestUnion
            {
               public sealed class ConcreteType1 : TestUnion;
               public sealed class ConcreteType2 : TestUnion;
               public sealed class ConcreteType3 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_switch_map_overload_when_types_have_no_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(ConcreteType1), typeof(ConcreteType2)])]
            public partial class TestUnion
            {
               public sealed class ConcreteType1 : TestUnion;
               public sealed class ConcreteType2 : TestUnion;
               public sealed class ConcreteType3 : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_not_generate_switch_map_overload_for_generic_union()
   {
      var source = """
         using System;
         using Thinktecture;

         // generics in attributes are not supported in C#

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(Result<T>.Success)])]
            public partial record Result<T>
            {
               public partial record Success(T Value) : Result<T>;
               public partial record Failure(string Error) : Result<T>;
               public partial record Pending : Result<T>;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source,
                                                                     [typeof(UnionAttribute).Assembly],
                                                                     ["'Thinktecture.Tests.Result<T>.Success': an attribute argument cannot use type parameters"]);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.Result`1.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_switch_map_overload_with_inheritance_hierarchy()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(BaseType)])]
            public partial class TestUnion
            {
               public abstract class BaseType : TestUnion;
               public sealed class ConcreteType1 : BaseType;
               public sealed class ConcreteType2 : BaseType;
               public sealed class OtherType : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_switch_map_overload_with_complex_inheritance_hierarchy()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(BaseType1), typeof(ConcreteType4)])]
            public partial class TestUnion
            {
               public abstract class BaseType1 : TestUnion;
               public sealed class ConcreteType1 : BaseType1;
               public class ConcreteType2 : BaseType1
               {
                  public sealed class ConcreteType2_1 : ConcreteType2;
               }

               public abstract class BaseType2 : TestUnion;
               public sealed class ConcreteType3 : BaseType2;
               public class ConcreteType4 : BaseType2
               {
                  public sealed class ConcreteType4_1 : ConcreteType4;
               }

               public sealed class DirectType : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_empty_switch_map_overload_when_no_types_match()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union]
            [UnionSwitchMapOverload(StopAt = [typeof(NonExistentType)])]
            public partial class TestUnion
            {
               public sealed class ConcreteType1 : TestUnion;
               public sealed class ConcreteType2 : TestUnion;
            }

            public class NonExistentType
            {
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_generate_switch_map_overload_respecting_partial_methods_setting()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(
               SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
               MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
            [UnionSwitchMapOverload(StopAt = [typeof(BaseType)])]
            public partial class TestUnion
            {
               public abstract class BaseType : TestUnion;
               public sealed class ConcreteType1 : BaseType;
               public sealed class ConcreteType2 : BaseType;
               public sealed class OtherType : TestUnion;
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestUnion.RegularUnion.g.cs");
   }

   [Fact]
   public async Task Should_handle_derived_types()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
            public partial class PlaceId
            {
               public class Unknown : PlaceId
               {
                  public static readonly Unknown Instance = new();

                  private Unknown()
                  {
                  }
               }

               [ValueObject<int>]
               public partial class CountryId : PlaceId;

               public abstract class AbstractRegionId : PlaceId
               {
                  private AbstractRegionId()
                  {
                  }

                  public sealed class SpecialRegionId : AbstractRegionId;
               }

               public class RegionId : PlaceId
               {
                  private RegionId()
                  {
                  }

                  public sealed class InnerPlaceId : PlaceId;

                  public sealed class InnerRegionId : RegionId;

                  public class InnerRegionId2 : object;
               }
            }
         }
         """;
      var outputs = GetGeneratedOutputs<RegularUnionSourceGenerator>(source, typeof(UnionAttribute).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.PlaceId.RegularUnion.g.cs");
   }
}
