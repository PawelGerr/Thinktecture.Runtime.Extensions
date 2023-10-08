using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SuppressMessage("ReSharper", "TTRESG100")]
[SmartEnum<string>(IsValidatable = true)]
public sealed partial class EmptyEnum
{
}
