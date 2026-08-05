namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<EmptyStateStruct, EmptyStateStruct>(
   T1IsStateless = true,
   T1Name = "First",
   T2Name = "Second",
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_stateless_emptystatestruct_emptystatestruct;
