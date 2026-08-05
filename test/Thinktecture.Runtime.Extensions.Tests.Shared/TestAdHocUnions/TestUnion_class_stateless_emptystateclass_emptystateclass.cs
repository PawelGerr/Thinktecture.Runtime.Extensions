namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<EmptyStateClass, EmptyStateClass>(
   T1IsStateless = true,
   T1Name = "First",
   T2Name = "Second",
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_stateless_emptystateclass_emptystateclass;
