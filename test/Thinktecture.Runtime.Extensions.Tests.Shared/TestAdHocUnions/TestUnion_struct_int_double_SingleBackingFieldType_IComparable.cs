using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, double>(SingleBackingFieldType = typeof(IComparable))]
public partial class TestUnion_struct_int_double_SingleBackingFieldType_IComparable;
