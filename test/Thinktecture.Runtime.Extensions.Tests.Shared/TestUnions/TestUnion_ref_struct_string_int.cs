namespace Thinktecture.Runtime.Tests.TestUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public ref partial struct TestUnion_ref_struct_string_int;
