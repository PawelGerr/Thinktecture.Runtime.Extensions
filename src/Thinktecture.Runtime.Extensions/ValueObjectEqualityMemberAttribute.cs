namespace Thinktecture;

/// <summary>
/// Marks the member for equality comparison.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ValueObjectEqualityMemberAttribute : Attribute
{
   /// <summary>
   /// A field or property defining the <see cref="IEqualityComparer{T}"/> to use.
   /// Example: "System.StringComparer.OrdinalIgnoreCase"
   /// </summary>
   public string? EqualityComparer { get; set; }

   /// <summary>
   /// A field or property defining the <see cref="IComparer{T}"/> to use.
   /// </summary>
   public string? Comparer { get; set; }
}
