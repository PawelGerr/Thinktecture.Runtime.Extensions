namespace Thinktecture;

/// <summary>
/// Marks the member for equality comparison and provides an <see cref="IComparer{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
// ReSharper disable once UnusedTypeParameter
public sealed class ValueObjectMemberComparerAttribute<T, TMember> : Attribute
   where T : IComparerAccessor<TMember>
{
}
