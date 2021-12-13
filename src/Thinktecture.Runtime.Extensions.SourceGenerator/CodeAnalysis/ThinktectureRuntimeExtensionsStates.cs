namespace Thinktecture.CodeAnalysis;

public readonly struct ThinktectureRuntimeExtensionsStates
{
   public readonly EnumSourceGeneratorState? EnumState;
   public readonly ValueObjectSourceGeneratorState? ValueObjectState;

   public bool IsEmpty => EnumState is null && ValueObjectState is null;

   public ThinktectureRuntimeExtensionsStates(EnumSourceGeneratorState? enumState, ValueObjectSourceGeneratorState? valueObjectState)
   {
      EnumState = enumState;
      ValueObjectState = valueObjectState;
   }
}
