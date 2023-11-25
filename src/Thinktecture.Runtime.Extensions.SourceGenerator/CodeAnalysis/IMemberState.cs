namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>, IMemberInformation, IHashCodeComputable
{
   ArgumentName ArgumentName { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   bool IsNullableStruct { get; }
   NullableAnnotation NullableAnnotation { get; }
}
