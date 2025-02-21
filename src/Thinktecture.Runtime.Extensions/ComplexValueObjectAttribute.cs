namespace Thinktecture;

/// <summary>
/// Marks the type as a Value Object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ComplexValueObjectAttribute : ValueObjectAttributeBase
{
   /// <summary>
   /// Defines the <see cref="StringComparison"/>.
   /// Default <see cref="StringComparison"/> is <see cref="StringComparison.OrdinalIgnoreCase"/>.
   /// </summary>
   public StringComparison DefaultStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;
}
