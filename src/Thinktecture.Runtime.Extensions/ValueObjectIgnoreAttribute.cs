using System;

namespace Thinktecture;

/// <summary>
/// Makes the member invisible to source generator.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ValueObjectIgnoreAttribute : Attribute
{
}