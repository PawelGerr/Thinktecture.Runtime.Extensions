using System;

namespace Thinktecture.Runtime.Tests.TestUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int, bool, Guid, char>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                                      MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_string_int_bool_guid_char;