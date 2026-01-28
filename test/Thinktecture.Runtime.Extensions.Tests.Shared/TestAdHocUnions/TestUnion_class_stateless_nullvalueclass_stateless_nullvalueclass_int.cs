namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<NullValueClass, NullValueClass, int>(
   T1IsStateless = true,
   T2IsStateless = true,
   T1Name = "NullValueClass1",
   T2Name = "NullValueClass2")]
public partial class TestUnion_class_stateless_nullvalueclass_stateless_nullvalueclass_int;
