using System;

namespace Thinktecture.Runtime.Tests.EnumExtensionsTests;

public class GetValidValue
{
   private enum MyIntEnum
   {
      None = 0,
      A = 1,
      B = 2,
      Negative = -5
   }

   [Flags]
   private enum FlagsNoCombined
   {
      None = 0,
      A = 1,
      B = 2,
      C = 4
   }

   [Flags]
   private enum FlagsWithCombined
   {
      None = 0,
      A = 1,
      B = 2,
      C = 4,
      AB = A | B
   }

   private enum ByteEnum : byte
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum SByteEnum : sbyte
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum ShortEnum : short
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum UShortEnum : ushort
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum UIntEnum : uint
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum LongEnum : long
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   private enum ULongEnum : ulong
   {
      None = 0,
      Value1 = 1,
      Value2 = 2
   }

   [Fact]
   public void GetValidValue_returns_item_for_defined_value()
   {
      0.GetValidValue<MyIntEnum>().Should().Be(MyIntEnum.None);
      1.GetValidValue<MyIntEnum>().Should().Be(MyIntEnum.A);
      2.GetValidValue<MyIntEnum>().Should().Be(MyIntEnum.B);
      (-5).GetValidValue<MyIntEnum>().Should().Be(MyIntEnum.Negative);
   }

   [Fact]
   public void GetValidValue_returns_null_for_undefined_value()
   {
      999.GetValidValue<MyIntEnum>().Should().BeNull();
   }

   [Fact]
   public void GetValidValue_returns_null_for_invalid_flags_combination_value()
   {
      // 3 is not a declared value in FlagsNoCombined
      42.GetValidValue<FlagsNoCombined>().Should().BeNull();
   }

   [Fact]
   public void GetValidValue_returns_value_for_unnamed_flags_combination_value()
   {
      // 3 is not a declared value in FlagsNoCombined
      3.GetValidValue<FlagsNoCombined>().Should().Be(FlagsNoCombined.A | FlagsNoCombined.B);
   }

   [Fact]
   public void GetValidValue_returns_named_flags_combination_value()
   {
      // 3 equals FlagsWithCombined.AB which is declared
      3.GetValidValue<FlagsWithCombined>().Should().Be(FlagsWithCombined.AB);
   }

   [Fact]
   public void Should_handle_byte_enum()
   {
      ((byte)1).GetValidValue<ByteEnum>().Should().Be(ByteEnum.Value1);
      ((byte)2).GetValidValue<ByteEnum>().Should().Be(ByteEnum.Value2);
      ((byte)99).GetValidValue<ByteEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_sbyte_enum()
   {
      ((sbyte)1).GetValidValue<SByteEnum>().Should().Be(SByteEnum.Value1);
      ((sbyte)2).GetValidValue<SByteEnum>().Should().Be(SByteEnum.Value2);
      ((sbyte)99).GetValidValue<SByteEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_short_enum()
   {
      ((short)1).GetValidValue<ShortEnum>().Should().Be(ShortEnum.Value1);
      ((short)2).GetValidValue<ShortEnum>().Should().Be(ShortEnum.Value2);
      ((short)99).GetValidValue<ShortEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_ushort_enum()
   {
      ((ushort)1).GetValidValue<UShortEnum>().Should().Be(UShortEnum.Value1);
      ((ushort)2).GetValidValue<UShortEnum>().Should().Be(UShortEnum.Value2);
      ((ushort)99).GetValidValue<UShortEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_uint_enum()
   {
      ((uint)1).GetValidValue<UIntEnum>().Should().Be(UIntEnum.Value1);
      ((uint)2).GetValidValue<UIntEnum>().Should().Be(UIntEnum.Value2);
      ((uint)99).GetValidValue<UIntEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_long_enum()
   {
      ((long)1).GetValidValue<LongEnum>().Should().Be(LongEnum.Value1);
      ((long)2).GetValidValue<LongEnum>().Should().Be(LongEnum.Value2);
      ((long)99).GetValidValue<LongEnum>().Should().BeNull();
   }

   [Fact]
   public void Should_handle_ulong_enum()
   {
      ((ulong)1).GetValidValue<ULongEnum>().Should().Be(ULongEnum.Value1);
      ((ulong)2).GetValidValue<ULongEnum>().Should().Be(ULongEnum.Value2);
      ((ulong)99).GetValidValue<ULongEnum>().Should().BeNull();
   }
}
