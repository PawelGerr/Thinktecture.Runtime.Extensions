using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(typeof(string), typeof(int), typeof(List<int>),
            SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
            MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_int_string_listOfInts;
