namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class ComplexValueObjectWithMembersDisallowingDefaultValues
{
   // The interface on a reference type is intentional: the tests verify that only value-type members
   // trigger the required-member rule.
#pragma warning disable TTRESG110
   public class ClassDisallowingDefaultValues : IDisallowDefaultValue;
#pragma warning restore TTRESG110

   // ReSharper disable once DefaultStructEqualityIsUsed.Global
   public struct StructDisallowingDefaultValues : IDisallowDefaultValue;

   public readonly ClassDisallowingDefaultValues NonNullableReferenceType;
   public readonly ClassDisallowingDefaultValues? NullableReferenceType;
   public readonly StructDisallowingDefaultValues NonNullableStruct;
   public readonly StructDisallowingDefaultValues? NullableStruct;
}
