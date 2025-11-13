namespace Thinktecture.Runtime.Tests.TestRegularUnions;

// ReSharper disable once InconsistentNaming
[Union]
public partial class TestUnion_with_empty_derived_type
{
   public sealed class Child1 : TestUnion_with_empty_derived_type;
}
