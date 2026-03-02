namespace Thinktecture.CodeAnalysis;

public interface ITypeKindInformation
{
   bool IsReferenceType { get; }
   bool IsValueType { get; }
   bool IsRecord { get; }
   bool IsTypeParameter { get; }
}
