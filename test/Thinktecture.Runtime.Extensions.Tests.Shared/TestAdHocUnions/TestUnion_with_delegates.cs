using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<Func<int, string>, Action<int>>]
public partial class TestUnion_with_delegates;
