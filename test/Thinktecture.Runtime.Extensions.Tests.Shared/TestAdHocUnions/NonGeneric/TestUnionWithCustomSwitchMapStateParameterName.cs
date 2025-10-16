namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

[AdHocUnion(
   typeof(string), typeof(int),
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   SwitchMapStateParameterName = "context")]
public partial class TestUnionWithCustomSwitchMapStateParameterName;
