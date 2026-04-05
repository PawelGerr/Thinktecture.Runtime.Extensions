namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<TypeParamRef1>]
public readonly partial struct GenericKeyBasedStructValueObject<T>
   where T : notnull;
