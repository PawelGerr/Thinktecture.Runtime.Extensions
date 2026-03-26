namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<TypeParamRef1, string>]
public partial struct TestUnion_generic_struct_notnull_constrained_TypeParamRef1_string<T>
   where T : notnull;
