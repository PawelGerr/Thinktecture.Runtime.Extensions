// TODO: Uncomment after TypeParamRef support is implemented for ValueObject (see typeparamref-for-smartenum-valueobject.md)
// Currently blocked by CS8968: C# prohibits type parameters as attribute type arguments.
// The TypeParamRef placeholder pattern (already used by unions) must be extended to ValueObject first.
//
// This struct variant exercises the DisallowsDefaultValue code path in KeyedValueObjectSourceGeneratorState,
// which is only relevant for struct value objects: IsValueType && (!AllowDefaultStructs || KeyMember.MayBeNull()).

// namespace Thinktecture.Runtime.Tests.TestValueObjects;
//
// [ValueObject<TypeParamRef1>]
// public readonly partial struct GenericKeyBasedStructValueObject<T>;
