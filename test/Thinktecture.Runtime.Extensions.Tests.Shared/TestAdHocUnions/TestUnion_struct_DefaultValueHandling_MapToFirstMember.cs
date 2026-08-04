namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// The "MaybeOf<T>" shape: 'default(TUnion)' represents the stateless first member (the "Null" state).

// ReSharper disable once InconsistentNaming
[Union<NullValueStruct, int>(
   T1IsStateless = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_MapToFirstMember_stateless_int;

// ReSharper disable once InconsistentNaming
[Union<NullValueStruct, int>(
   T1IsStateless = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember,
   UseSingleBackingField = true,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_MapToFirstMember_UseSingleBackingField;

// ReSharper disable once InconsistentNaming
[Union<EmptyFooStruct, Foo1>(
   SingleBackingFieldType = typeof(IFoo),
   T1IsStateless = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public readonly partial struct TestUnion_struct_MapToFirstMember_SingleBackingFieldType;

// Generic "MaybeOf<T>": the value member is the type parameter, the first member is the stateless "Null" marker.
// ReSharper disable once InconsistentNaming
[Union<NullValueStruct, TypeParamRef1>(
   T1IsStateless = true,
   DefaultValueHandling = UnionDefaultValueHandling.MapToFirstMember,
   SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
   MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_generic_struct_MapToFirstMember<T>;
