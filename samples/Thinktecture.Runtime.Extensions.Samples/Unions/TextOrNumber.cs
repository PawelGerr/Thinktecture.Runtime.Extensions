namespace Thinktecture.Unions;

[Union<string, int>(T1IsNullableReferenceType = true,
                    T1Name = "Text",
                    T2Name = "Number",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TextOrNumber;
