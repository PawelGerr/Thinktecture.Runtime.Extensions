namespace Thinktecture.CodeAnalysis;

public interface IMemberInformation : ITypeInformationWithNullability
{
   string Name { get; }
   SpecialType SpecialType { get; }
}
