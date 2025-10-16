namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int), typeof(string), typeof(string), typeof(int?),
   T4IsNullableReferenceType = true,
   T1Name = "Text",
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_with_same_types;
