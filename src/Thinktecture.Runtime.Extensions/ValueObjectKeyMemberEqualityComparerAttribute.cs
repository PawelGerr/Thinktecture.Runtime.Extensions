namespace Thinktecture;

/// <summary>
/// Defines the equality comparer for the key member.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
// ReSharper disable once UnusedTypeParameter
public sealed class ValueObjectKeyMemberEqualityComparerAttribute<TAccessor, TKey> : Attribute
   where TAccessor : IEqualityComparerAccessor<TKey>
   where TKey : notnull;
