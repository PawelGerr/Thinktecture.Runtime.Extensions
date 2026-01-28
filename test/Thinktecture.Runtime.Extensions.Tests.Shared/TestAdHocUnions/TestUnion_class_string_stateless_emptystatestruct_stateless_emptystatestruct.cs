namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, EmptyStateStruct, EmptyStateStruct>(
   T2IsStateless = true,
   T3IsStateless = true,
   T2Name = "EmptyState1",
   T3Name = "EmptyState2")]
public partial class TestUnion_class_string_stateless_emptystatestruct_stateless_emptystatestruct;
