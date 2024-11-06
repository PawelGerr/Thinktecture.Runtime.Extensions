using System;

namespace Thinktecture.Runtime.Tests.TestUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int, string, string, int?>(T4IsNullableReferenceType = true,
                                          T1Name = "Text",
                                          DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_with_same_types_case_sensitive;
