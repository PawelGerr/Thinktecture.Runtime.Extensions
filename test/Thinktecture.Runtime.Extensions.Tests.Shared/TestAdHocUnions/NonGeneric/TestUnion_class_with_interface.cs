using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(typeof(IReadOnlyList<string?>), typeof(int))]
public partial class TestUnion_class_with_interface;
