using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ValueObject<Guid>(KeyMemberKind = MemberKind.Property, KeyMemberAccessModifier = AccessModifier.Public, KeyMemberName = "Value")]
public partial class ValueObject_Generic_GuidBased<T>
   where T : notnull
{
   public T? AdditionalValue { get; private init; }
}
