using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[AdHocUnion(typeof(List<TypeParamRef1?>), typeof(string))]
public partial struct TestUnion_generic_struct_nullable_nested_TypeParamRef1_class_constraint<T>
   where T : class;
