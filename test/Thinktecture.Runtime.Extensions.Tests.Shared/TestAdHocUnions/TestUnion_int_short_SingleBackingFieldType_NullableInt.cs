namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, short>(SingleBackingFieldType = typeof(int?))]
public partial class TestUnion_int_short_SingleBackingFieldType_NullableInt;
