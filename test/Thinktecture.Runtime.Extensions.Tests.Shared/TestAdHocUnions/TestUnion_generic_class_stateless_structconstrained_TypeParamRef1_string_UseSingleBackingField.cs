namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<TypeParamRef1, string>(
   UseSingleBackingField = true,
   T1IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_generic_class_stateless_structconstrained_TypeParamRef1_string_UseSingleBackingField<T>
   where T : struct;
