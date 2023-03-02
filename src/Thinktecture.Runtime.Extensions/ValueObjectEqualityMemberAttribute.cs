namespace Thinktecture;

/// <summary>
/// Marks the member for equality comparison.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ValueObjectEqualityMemberAttribute : Attribute
{
}
