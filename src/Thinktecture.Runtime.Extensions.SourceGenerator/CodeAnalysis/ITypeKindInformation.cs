namespace Thinktecture.CodeAnalysis;

public interface ITypeKindInformation
{
   bool IsReferenceType { get; }
   bool IsStruct { get; }
   bool IsRecord { get; }
   bool IsTypeParameter { get; }
}
