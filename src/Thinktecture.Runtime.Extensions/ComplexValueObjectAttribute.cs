using System.Numerics;

namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComplexValueObjectAttribute : ValueObjectAttributeBase
{
   /// <summary>
   /// Indication whether and how the generator should generate the implementation of <see cref="IEqualityOperators{TSelf,TOther,TResult}"/>.
   ///
   /// This setting has no effect:
   /// - on non-keyed value objects (i.e. has more than 1 field/property)
   /// - if key-member is not an <see cref="IEqualityOperators{TSelf,TOther,TResult}"/> itself and has no corresponding operators (<c>op_Equality</c>, <c>op_Inequality</c>).
   /// </summary>
   public override OperatorsGeneration EqualityComparisonOperators { get; set; }
}
