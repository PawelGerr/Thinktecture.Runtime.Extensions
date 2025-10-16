namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int?),
   T1IsNullableReferenceType = true)]
public partial class TestUnion_class_nullable_string_nullable_int;
