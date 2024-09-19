namespace Thinktecture.Runtime.Tests.TestUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_string_int;
