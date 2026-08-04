namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(ValueMemberAccessModifier = AccessModifier.Private,
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_string_int_ValueMemberAccessModifier_private
{
   // Gates the struct Value getter (index check plus throw) under a non-public access modifier.
   public object? ReadValueInternally() => Value;
}
