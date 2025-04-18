namespace Thinktecture;

/// <summary>
/// Marks the member for equality comparison and provides an <see cref="IEqualityComparer{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Obsolete("Use 'MemberEqualityComparerAttribute' instead.")]
// ReSharper disable once UnusedTypeParameter
public sealed class ValueObjectMemberEqualityComparerAttribute<T, TMember> : Attribute
   where T : IEqualityComparerAccessor<TMember>;

/// <summary>
/// Marks the member for equality comparison and provides an <see cref="IEqualityComparer{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
// ReSharper disable once UnusedTypeParameter
public sealed class MemberEqualityComparerAttribute<T, TMember> : Attribute
   where T : IEqualityComparerAccessor<TMember>;
