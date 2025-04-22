namespace Thinktecture.CodeAnalysis;

public interface ITypeInformationProvider<out T>
   where T : ITypeFullyQualified, INamespaceAndName
{
   T Type { get; }
}
