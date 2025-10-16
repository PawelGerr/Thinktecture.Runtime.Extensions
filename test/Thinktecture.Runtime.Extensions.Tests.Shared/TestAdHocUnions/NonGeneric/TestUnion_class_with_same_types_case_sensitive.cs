using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?),
   T4IsNullableReferenceType = true,
   T1Name = "Text",
   DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_with_same_types_case_sensitive;
