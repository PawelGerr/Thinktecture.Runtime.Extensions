using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_string_int_case_sensitive;
