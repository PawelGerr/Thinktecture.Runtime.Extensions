using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int),
   DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_string_int_case_sensitive;
