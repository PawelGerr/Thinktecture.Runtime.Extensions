namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<EmptyStateStruct, EmptyStateStruct>(
   T2IsStateless = true,
   UseSingleBackingField = true,
   T1Name = "First",
   T2Name = "Second",
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_emptystatestruct_stateless_emptystatestruct_UseSingleBackingField;
