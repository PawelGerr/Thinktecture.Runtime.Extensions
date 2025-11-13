namespace Thinktecture;

/// <summary>
/// Defines the comparer for the key member.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
// ReSharper disable once UnusedTypeParameter
public sealed class KeyMemberComparerAttribute<TAccessor, TKey> : Attribute
   where TAccessor : IComparerAccessor<TKey>
   where TKey : notnull;
