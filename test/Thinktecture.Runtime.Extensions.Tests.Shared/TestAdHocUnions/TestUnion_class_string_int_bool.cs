namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int, bool>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                          MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_string_int_bool;
