namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class ComplexValueObjectWithMembersDisallowingDefaultValues
{
   public class ClassDisallowingDefaultValues : IDisallowDefaultValue;

   // ReSharper disable once DefaultStructEqualityIsUsed.Global
   public struct StructDisallowingDefaultValues : IDisallowDefaultValue;

   public readonly ClassDisallowingDefaultValues NonNullableReferenceType;
   public readonly ClassDisallowingDefaultValues? NullableReferenceType;
   public readonly StructDisallowingDefaultValues NonNullableStruct;
   public readonly StructDisallowingDefaultValues? NullableStruct;
}
