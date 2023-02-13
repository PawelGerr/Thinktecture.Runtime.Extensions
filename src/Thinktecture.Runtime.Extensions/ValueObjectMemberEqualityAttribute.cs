namespace Thinktecture;

/// <summary>
/// Marks the member for equality comparison and provides an <see cref="IEqualityComparer{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ValueObjectMemberEqualityAttribute<T, TMember> : Attribute
   where T : IEqualityComparerAccessor<TMember>
{
}
