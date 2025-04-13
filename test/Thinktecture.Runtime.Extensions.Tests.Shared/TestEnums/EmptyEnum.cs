using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TestEnums;

[SuppressMessage("ReSharper", "TTRESG100")]
[SmartEnum<string>(IsValidatable = true)]
public partial class EmptyEnum;
