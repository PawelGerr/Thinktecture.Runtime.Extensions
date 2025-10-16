using System;

namespace Thinktecture.Runtime.Tests.TestAdHocUnions.NonGeneric;

// ReSharper disable once InconsistentNaming
[AdHocUnion(
   typeof(string), typeof(int), typeof(bool), typeof(Guid), typeof(char),
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   DefaultStringComparison = StringComparison.Ordinal)]
public partial class TestUnion_class_string_int_bool_case_sensitive;
