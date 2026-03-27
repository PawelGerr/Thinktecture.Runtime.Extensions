namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(FactoryMethodGeneration = FactoryMethodGeneration.Always)]
public partial class TestUnion_class_string_int_with_FactoryMethodGeneration_Always;
