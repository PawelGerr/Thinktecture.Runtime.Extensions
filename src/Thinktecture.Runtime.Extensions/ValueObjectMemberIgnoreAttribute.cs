namespace Thinktecture;

/// <summary>
/// Makes the member invisible to source generator.
///
/// Please make sure that the corresponding member is immutable.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ValueObjectMemberIgnoreAttribute : Attribute
{
}
