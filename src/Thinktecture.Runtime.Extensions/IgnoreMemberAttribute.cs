namespace Thinktecture;

/// <summary>
/// Makes the member invisible to the source generator.
///
/// Please make sure that the corresponding member is immutable.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Obsolete("Use 'IgnoreMemberAttribute' instead.")]
public sealed class ValueObjectMemberIgnoreAttribute : Attribute;

/// <summary>
/// Makes the member invisible to the source generator.
///
/// Please make sure that the corresponding member is immutable.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class IgnoreMemberAttribute : Attribute;
