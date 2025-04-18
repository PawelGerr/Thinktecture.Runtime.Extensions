namespace Thinktecture;

/// <summary>
/// Marks an API as internal to Thinktecture.Runtime.Extension.
/// These APIs are not subject to the same compatibility standards as public APIs.
/// It may be changed or removed without notice in any release.
/// You should only use such APIs directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extension release.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
public sealed class ThinktectureRuntimeExtensionInternalAttribute : Attribute;
