using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int, bool, Guid, char>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                                      MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                                      DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_string_int_bool_case_sensitive;
