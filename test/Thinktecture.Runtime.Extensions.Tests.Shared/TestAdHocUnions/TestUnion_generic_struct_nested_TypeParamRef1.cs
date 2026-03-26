using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<TypeParamRef1, List<TypeParamRef1>>]
public partial struct TestUnion_generic_struct_nested_TypeParamRef1<T>;
