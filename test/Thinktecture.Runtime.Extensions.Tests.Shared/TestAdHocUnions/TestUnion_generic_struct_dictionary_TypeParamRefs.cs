using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<Dictionary<TypeParamRef1, TypeParamRef2>, string>]
public partial struct TestUnion_generic_struct_dictionary_TypeParamRefs<T1, T2>
   where T1 : notnull;
