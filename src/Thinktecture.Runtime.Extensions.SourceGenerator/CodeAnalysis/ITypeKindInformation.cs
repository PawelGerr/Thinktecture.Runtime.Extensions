namespace Thinktecture.CodeAnalysis;

public interface ITypeKindInformation
{
   bool IsReferenceType { get; }
   bool IsRecord { get; }
}
