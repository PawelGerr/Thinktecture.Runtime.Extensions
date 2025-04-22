namespace Thinktecture.CodeAnalysis;

public interface IHasGenerics
{
   IReadOnlyList<GenericTypeParameterState> GenericParameters { get; }
}
