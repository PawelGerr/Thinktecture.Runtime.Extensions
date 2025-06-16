namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
// ReSharper disable once InconsistentNaming
public partial class TestUnion_class_string_int_with_base_class : TestBaseClass;
