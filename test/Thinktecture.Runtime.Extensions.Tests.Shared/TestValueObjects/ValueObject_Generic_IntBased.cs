using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<int>(KeyMemberKind = MemberKind.Property, KeyMemberAccessModifier = AccessModifier.Public, KeyMemberName = "Value")]
public partial class ValueObject_Generic_IntBased<T>
   where T : IEquatable<T>
{
   public T? AdditionalValue { get; private init; }
}
