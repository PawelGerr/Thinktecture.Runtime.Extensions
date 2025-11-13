namespace Thinktecture.CodeAnalysis;

public interface IHasGenerics
{
   ImmutableArray<GenericTypeParameterState> GenericParameters { get; }
}
