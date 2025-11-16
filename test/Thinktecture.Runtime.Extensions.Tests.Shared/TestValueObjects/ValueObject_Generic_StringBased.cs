namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
[ValueObject<string>(KeyMemberKind = MemberKind.Property, KeyMemberAccessModifier = AccessModifier.Public, KeyMemberName = "Value")]
public partial class ValueObject_Generic_StringBased<T>
   where T : class
{
   public T? AdditionalValue { get; private init; }
}
