namespace Thinktecture.CodeAnalysis;

public readonly record struct ThinktectureRuntimeExtensionsStates(EnumSourceGeneratorState? EnumState, ValueObjectSourceGeneratorState? ValueObjectState)
{
   public bool IsEmpty => EnumState is null && ValueObjectState is null;
}
