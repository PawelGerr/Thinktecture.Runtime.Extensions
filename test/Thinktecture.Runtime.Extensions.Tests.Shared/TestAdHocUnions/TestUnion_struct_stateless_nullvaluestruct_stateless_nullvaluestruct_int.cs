namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<NullValueStruct, NullValueStruct, int>(
   T1IsStateless = true,
   T2IsStateless = true,
   T1Name = "NullValue1",
   T2Name = "NullValue2")]
public partial struct TestUnion_struct_stateless_nullvaluestruct_stateless_nullvaluestruct_int;
