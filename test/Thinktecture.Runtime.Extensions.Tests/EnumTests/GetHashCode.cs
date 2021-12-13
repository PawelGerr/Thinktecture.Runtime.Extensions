using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key()
   {
      var hashCode = TestEnum.Item1.GetHashCode();
      var typeHashCode = typeof(TestEnum).GetHashCode();
      var keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(TestEnum.Item1.Key);
      hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);

      hashCode = ExtensibleTestEnum.Item1.GetHashCode();
      typeHashCode = typeof(ExtensibleTestEnum).GetHashCode();
      keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ExtensibleTestEnum.Item1.Key);
      hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);
   }

   [Fact]
   public void Should_return_hashcode_of_the_base_enum_type_plus_key()
   {
      var baseTypeHashCode = typeof(ExtensibleTestEnum).GetHashCode();

      var hashCode = ExtendedTestEnum.Item1.GetHashCode();
      var keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ExtendedTestEnum.Item1.Key);
      hashCode.Should().Be((baseTypeHashCode * 397) ^ keyHashCode);

      hashCode = ExtendedSiblingTestEnum.Item1.GetHashCode();
      keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ExtendedSiblingTestEnum.Item1.Key);
      hashCode.Should().Be((baseTypeHashCode * 397) ^ keyHashCode);

      hashCode = DifferentAssemblyExtendedTestEnum.Item1.GetHashCode();
      keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(DifferentAssemblyExtendedTestEnum.Item1.Key);
      hashCode.Should().Be((baseTypeHashCode * 397) ^ keyHashCode);
   }

   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key_for_structs()
   {
      var hashCode = StructIntegerEnum.Item1.GetHashCode();

      var typeHashCode = typeof(StructIntegerEnum).GetHashCode();
      var keyHashCode = StructIntegerEnum.Item1.Key.GetHashCode();
      hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);
   }
}