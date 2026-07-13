namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

[Union<string, int>(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    SwitchMapStateParameterName = "x")]
public partial class TestUnionWithXSwitchMapStateParameterName;
