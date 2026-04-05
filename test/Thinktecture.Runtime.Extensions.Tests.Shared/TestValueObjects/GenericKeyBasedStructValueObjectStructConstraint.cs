namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<TypeParamRef1>]
public readonly partial struct GenericKeyBasedStructValueObjectStructConstraint<T>
   where T : struct;
