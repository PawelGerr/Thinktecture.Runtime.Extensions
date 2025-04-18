namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    SwitchMapStateParameterName = "context")]
public partial class TestUnionWithCustomSwitchMapStateParameterName;
