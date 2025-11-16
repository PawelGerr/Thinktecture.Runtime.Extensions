using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<int>(KeyMemberKind = MemberKind.Property, KeyMemberAccessModifier = AccessModifier.Public, KeyMemberName = "Value")]
public partial struct StructValueObject_Generic_IntBased<T>
   where T : IEquatable<T>
{
   public T? AdditionalValue { get; private init; }
}
