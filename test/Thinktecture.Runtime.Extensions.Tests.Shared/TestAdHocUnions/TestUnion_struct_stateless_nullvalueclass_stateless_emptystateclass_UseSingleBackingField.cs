namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<NullValueClass, EmptyStateClass>(
   UseSingleBackingField = true,
   T1IsStateless = true,
   T2IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_stateless_nullvalueclass_stateless_emptystateclass_UseSingleBackingField;
