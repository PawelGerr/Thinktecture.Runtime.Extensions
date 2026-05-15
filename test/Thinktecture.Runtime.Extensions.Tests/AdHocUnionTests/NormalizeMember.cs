#nullable enable
using System;
using System.Threading;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
public class NormalizeMember
{
   public class PublicCtorPath
   {
      [Fact]
      public void Should_normalize_string_value_in_public_constructor()
      {
         var union = new NormalizingUnion("  Hello  ");

         union.AsString.Should().Be("hello");
         union.Value.Should().Be("hello");
      }

      [Fact]
      public void Should_normalize_int_value_in_public_constructor()
      {
         var union = new NormalizingUnion(7);

         union.AsInt32.Should().Be(14);
         union.Value.Should().Be(14);
      }

      [Fact]
      public void Should_reflect_normalized_value_in_ToString()
      {
         var union = new NormalizingUnion("  HELLO  ");

         union.ToString().Should().Be("hello");
      }

      [Fact]
      public void Should_use_normalized_value_for_Equals_and_GetHashCode()
      {
         var a = new NormalizingUnion("  HELLO  ");
         var b = new NormalizingUnion("hello");

         a.Equals(b).Should().BeTrue();
         a.GetHashCode().Should().Be(b.GetHashCode());
      }
   }

   public class ImplicitConversionPath
   {
      [Fact]
      public void Should_normalize_through_implicit_conversion_from_string()
      {
         NormalizingUnion union = "  Hello  ";

         union.AsString.Should().Be("hello");
      }

      [Fact]
      public void Should_normalize_through_implicit_conversion_from_int()
      {
         NormalizingUnion union = 5;

         union.AsInt32.Should().Be(10);
      }
   }

   public class FactoryDelegatingToCtorPath
   {
      // Counter==0 + forced factory: interface member triggers factory generation for ALL members,
      // but both members are unique (counter==0) so Normalize lives in the public ctor. The factory
      // delegates to the ctor; verifies Normalize fires exactly once and does NOT double-fire.
      [Fact]
      public void Should_normalize_via_ctor_when_factory_delegates_for_counter_zero_member()
      {
         var union = FactoryThroughCtorUnion.CreateString("  Hello  ");

         union.AsString.Should().Be("hello");
         FactoryThroughCtorUnion.NormalizeStringCallCount.Should().Be(1);
      }
   }

   public class FactoryPathForDuplicateMember
   {
      // CRITICAL: counter != 0 member -- Normalize lives in the factory body and the private
      // indexed ctor must NOT also normalize. This test guards the double-fire regression
      // explicitly called out in the feature plan's Risks section.
      [Fact]
      public void Should_normalize_through_factory_for_duplicate_member_exactly_once()
      {
         var contact = DuplicateFactoryUnion.CreateEmail("  X@Y.COM  ");

         contact.AsEmail.Should().Be("x@y.com");
         DuplicateFactoryUnion.NormalizeEmailCallCount.Should().Be(1);
      }
   }

   public class StatelessMember
   {
      [Fact]
      public void Should_not_normalize_stateless_member()
      {
         // Stateless members are constructed via the ctor accepting the stateless type instance.
         // The fact that this compiles and creates an instance is the proof: declaring an
         // implementation for NormalizeMissing would fail compilation because no
         // matching partial method exists for the stateless member.
         var union = new StatelessAwareUnion(new Missing());

         union.IsMissing.Should().BeTrue();
         union.IsString.Should().BeFalse();
      }

      [Fact]
      public void Should_still_normalize_non_stateless_member_in_stateless_union()
      {
         var union = new StatelessAwareUnion("  WORLD  ");

         union.AsString.Should().Be("world");
      }
   }

   public class GenericUnion
   {
      [Fact]
      public void Should_normalize_value_with_TypeParamRef1_member_for_int_type_parameter()
      {
         var box = new Box<int>(5);

         box.AsT.Should().Be(10);
      }

      [Fact]
      public void Should_normalize_string_member_of_generic_union()
      {
         var box = new Box<int>("  Hi  ");

         box.AsString.Should().Be("hi");
      }
   }

   public class ThrowingNormalize
   {
      [Fact]
      public void Should_propagate_exception_thrown_by_Normalize_from_constructor()
      {
         Action act = () => new ThrowingUnion("  bad  ");

         act.Should().Throw<InvalidOperationException>().WithMessage("Rejected by normalizer.");
      }

      [Fact]
      public void Should_propagate_exception_thrown_by_Normalize_from_factory()
      {
         Action act = () => ContactUnionThrowing.CreateEmail("  bad  ");

         act.Should().Throw<InvalidOperationException>().WithMessage("Email rejected.");
      }
   }

   public class WithoutNormalizeImplementation
   {
      // Empty partial method => zero-cost no-op. Values pass through unchanged.
      [Fact]
      public void Should_store_value_unchanged_when_no_Normalize_implementation_present()
      {
         var union = new PassthroughUnion("  not trimmed  ");

         union.AsString.Should().Be("  not trimmed  ");
      }
   }

   public class SingleBackingFieldTypeWithValueTypeMember
   {
      // Verifies Normalize fires on the typed value BEFORE the boxing assignment to _obj.
      [Fact]
      public void Should_normalize_value_type_before_boxing_to_typed_backing_field()
      {
         var union = new ComparableUnion(3);

         union.AsInt32.Should().Be(6);
         union.Value.Should().Be(6);
      }
   }
}

// ----- Test types -----

[Union<string, int>]
public partial class NormalizingUnion
{
   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
   }

   static partial void NormalizeInt32(ref int @int32)
   {
      @int32 *= 2;
   }
}

public interface INormalizeFoo
{
   string Bar { get; }
}

// Forces factory generation for all members via the interface trigger while keeping both members
// at counter==0. The factory delegates to the public ctor; Normalize lives in the ctor. Owns a
// private static counter so no other test type can race on it.
[Union<string, INormalizeFoo>]
public partial class FactoryThroughCtorUnion
{
   private static int _normalizeStringCallCount;

   public static int NormalizeStringCallCount => _normalizeStringCallCount;

   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
      Interlocked.Increment(ref _normalizeStringCallCount);
   }
}

// Both members have counter != 0 (T1=1, T2=2). Normalize lives in the factory body, not in the
// private indexed ctor. Owns a private static counter so no other test type can race on it.
[Union<string, string>(T1Name = "Name", T2Name = "Email")]
public partial class DuplicateFactoryUnion
{
   private static int _normalizeEmailCallCount;

   public static int NormalizeEmailCallCount => _normalizeEmailCallCount;

   static partial void NormalizeName(ref string @name)
   {
      @name = @name.Trim().ToLowerInvariant();
   }

   static partial void NormalizeEmail(ref string @email)
   {
      @email = @email.Trim().ToLowerInvariant();
      Interlocked.Increment(ref _normalizeEmailCallCount);
   }
}

public sealed class Missing;

[Union<Missing, string>(T1IsStateless = true)]
public partial class StatelessAwareUnion
{
   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
   }
}

[Union<Thinktecture.TypeParamRef1, string>]
public partial class Box<T>
{
   static partial void NormalizeT(ref T @t)
   {
      if (@t is int i)
         @t = (T)(object)(i * 2);
   }

   static partial void NormalizeString(ref string @string)
   {
      @string = @string.Trim().ToLowerInvariant();
   }
}

[Union<string, int>]
public partial class ThrowingUnion
{
   static partial void NormalizeString(ref string @string)
   {
      throw new InvalidOperationException("Rejected by normalizer.");
   }
}

[Union<string, string>(T1Name = "Name", T2Name = "Email")]
public partial class ContactUnionThrowing
{
   static partial void NormalizeName(ref string @name)
   {
      @name = @name.Trim();
   }

   static partial void NormalizeEmail(ref string @email)
   {
      throw new InvalidOperationException("Email rejected.");
   }
}

[Union<string, int>]
public partial class PassthroughUnion;

[Union<int, double>(SingleBackingFieldType = typeof(IComparable))]
public partial class ComparableUnion
{
   static partial void NormalizeInt32(ref int @int32)
   {
      @int32 *= 2;
   }

   static partial void NormalizeDouble(ref double @double)
   {
      @double *= 2.0;
   }
}
