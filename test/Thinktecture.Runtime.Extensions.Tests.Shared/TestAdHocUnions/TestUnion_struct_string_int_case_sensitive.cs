using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(DefaultStringComparison = StringComparison.Ordinal)]
public partial struct TestUnion_struct_string_int_case_sensitive;
