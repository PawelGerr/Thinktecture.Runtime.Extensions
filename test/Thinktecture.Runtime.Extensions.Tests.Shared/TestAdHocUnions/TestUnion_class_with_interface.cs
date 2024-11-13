using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<IReadOnlyList<string>, int>]
public partial class TestUnion_class_with_interface;
