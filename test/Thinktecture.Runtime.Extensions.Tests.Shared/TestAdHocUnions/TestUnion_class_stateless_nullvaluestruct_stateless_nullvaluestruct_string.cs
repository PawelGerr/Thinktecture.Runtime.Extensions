namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<NullValueStruct, NullValueStruct, string>(
   T1IsStateless = true,
   T2IsStateless = true,
   T1Name = "NullValue1",
   T2Name = "NullValue2")]
public partial class TestUnion_class_stateless_nullvaluestruct_stateless_nullvaluestruct_string;
