namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   t2: typeof(int),
   t1: typeof(string),
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_string_int;
