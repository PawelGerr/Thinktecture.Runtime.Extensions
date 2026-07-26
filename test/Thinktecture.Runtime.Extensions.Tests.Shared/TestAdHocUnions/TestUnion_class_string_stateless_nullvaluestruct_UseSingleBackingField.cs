namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, NullValueStruct>(
   UseSingleBackingField = true,
   T2IsStateless = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_string_stateless_nullvaluestruct_UseSingleBackingField;
