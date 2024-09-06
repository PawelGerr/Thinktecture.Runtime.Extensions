using System;

namespace Thinktecture.Runtime.Tests.TestUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int, bool, Guid>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                                MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                                DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_string_int_bool_guid_case_sensitive;
