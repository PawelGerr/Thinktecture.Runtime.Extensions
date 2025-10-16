using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int),
   DefaultStringComparison = StringComparison.Ordinal)]
public partial struct TestUnion_struct_string_int_case_sensitive;
