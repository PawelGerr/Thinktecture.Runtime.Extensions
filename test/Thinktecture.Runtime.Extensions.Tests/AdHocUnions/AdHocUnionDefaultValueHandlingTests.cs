#nullable enable
using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnions;

// Local unions so the tests are self-contained. 'default(TUnion)' maps to the stateless first member (T1).
[Union<NullValueStruct, int>(
   T1IsStateless = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember)]
public partial struct MaybeInt;

// Regression reference: the default behavior (Disallow) must keep throwing on 'default(TUnion)'.
[Union<NullValueStruct, int>(T1IsStateless = true)]
public partial struct DisallowMaybeInt;

// UseSingleBackingField: the stateless struct T1 is stored as a cached boxed default. 'default(TUnion).Value'
// must return that boxed T1, not null (the collapsed getter must not read the null '_obj' at index 0).
[Union<EmptyFooStruct, Foo1>(
   T1IsStateless = true,
   UseSingleBackingField = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember)]
public partial struct MaybeFoo_SingleBackingField;

// SingleBackingFieldType: same as above, but the backing field and 'Value' are typed as 'IFoo'.
[Union<EmptyFooStruct, Foo1>(
   T1IsStateless = true,
   SingleBackingFieldType = typeof(IFoo),
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember)]
public partial struct MaybeFoo_SingleBackingFieldType;

public class AdHocUnionDefaultValueHandlingTests
{
   [Fact]
   public void Should_map_default_to_first_member_when_MapToFirstMember()
   {
      MaybeInt value = default;

      value.IsNullValueStruct.Should().BeTrue();
      value.IsInt32.Should().BeFalse();
   }

   [Fact]
   public void Should_treat_new_identical_to_default_when_MapToFirstMember()
   {
      var byDefault = default(MaybeInt);
      var byNew = new MaybeInt();

      byNew.Should().Be(byDefault);
      byNew.IsNullValueStruct.Should().BeTrue();
   }

   [Fact]
   public void Should_equal_a_constructed_first_member_when_MapToFirstMember()
   {
      MaybeInt byDefault = default;
      MaybeInt byMember = new NullValueStruct();

      byDefault.Equals(byMember).Should().BeTrue();
      (byDefault == byMember).Should().BeTrue();
      byDefault.GetHashCode().Should().Be(byMember.GetHashCode());
   }

   [Fact]
   public void Should_not_equal_the_second_member_when_MapToFirstMember()
   {
      MaybeInt byDefault = default;
      MaybeInt second = 42;

      byDefault.Equals(second).Should().BeFalse();
      (byDefault == second).Should().BeFalse();
   }

   [Fact]
   public void Should_return_first_member_from_Value_on_default_when_MapToFirstMember()
   {
      MaybeInt value = default;

      value.Value.Should().Be(new NullValueStruct());
   }

   [Fact]
   public void Should_return_first_member_from_AsT1_on_default_when_MapToFirstMember()
   {
      MaybeInt value = default;

      value.AsNullValueStruct.Should().Be(new NullValueStruct());
   }

   [Fact]
   public void Should_switch_to_first_member_arm_on_default_when_MapToFirstMember()
   {
      MaybeInt value = default;

      var result = value.Switch(
         nullValueStruct: static _ => "null",
         int32: static _ => "int");

      result.Should().Be("null");
   }

   [Fact]
   public void Should_map_to_first_member_arm_on_default_when_MapToFirstMember()
   {
      MaybeInt value = default;

      var result = value.Map(
         nullValueStruct: "null",
         int32: "int");

      result.Should().Be("null");
   }

   [Fact]
   public void Should_return_first_member_name_from_ToString_on_default_when_MapToFirstMember()
   {
      MaybeInt value = default;

      value.ToString().Should().Be(new NullValueStruct().ToString());
   }

   [Fact]
   public void Should_throw_on_default_Value_when_Disallow()
   {
      DisallowMaybeInt value = default;

      value.Invoking(static v => v.Value)
           .Should().Throw<System.InvalidOperationException>();
   }

   [Fact]
   public void Should_return_boxed_first_member_from_Value_on_default_when_UseSingleBackingField()
   {
      MaybeFoo_SingleBackingField byDefault = default;
      MaybeFoo_SingleBackingField byNew = new();
      MaybeFoo_SingleBackingField byMember = new EmptyFooStruct();

      byDefault.Value.Should().NotBeNull();
      byDefault.Value.Should().Be(new EmptyFooStruct());
      byNew.Value.Should().Be(byMember.Value);
      byDefault.Value.Should().Be(byMember.Value);
   }

   [Fact]
   public void Should_return_boxed_first_member_from_Value_on_default_when_SingleBackingFieldType()
   {
      MaybeFoo_SingleBackingFieldType byDefault = default;
      MaybeFoo_SingleBackingFieldType byNew = new();
      MaybeFoo_SingleBackingFieldType byMember = new EmptyFooStruct();

      byDefault.Value.Should().NotBeNull();
      byDefault.Value.Should().Be(new EmptyFooStruct());
      byNew.Value.Should().Be(byMember.Value);
      byDefault.Value.Should().Be(byMember.Value);
   }
}
