using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[AdHocUnion(typeof(int), typeof(string), typeof(List<int>), typeof(bool), typeof(int?), SkipEqualityComparison = true)]
public partial class AdHocUnionNonGeneric_with_SkipEqualityComparison;
