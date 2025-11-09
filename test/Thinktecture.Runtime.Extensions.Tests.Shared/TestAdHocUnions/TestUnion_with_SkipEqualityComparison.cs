using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, string, List<int>, bool, int?>(SkipEqualityComparison = true)]
public partial class AdHocUnion_with_SkipEqualityComparison;
