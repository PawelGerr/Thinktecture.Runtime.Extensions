using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<List<TypeParamRef1>, TypeParamRef1[]>(SingleBackingFieldType = typeof(IEnumerable<TypeParamRef1>))]
public partial class TestUnion_generic_with_TypeParamRef<T>
   where T : notnull;
