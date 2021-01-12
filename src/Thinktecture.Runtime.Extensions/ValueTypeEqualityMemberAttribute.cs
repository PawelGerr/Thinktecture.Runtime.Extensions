using System;

namespace Thinktecture
{
   /// <summary>
   /// Marks the member for equality comparison.
   /// </summary>
   [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
   public class ValueTypeEqualityMemberAttribute : Attribute
   {
      /// <summary>
      /// A field or property defining the comparer to use.
      /// Example: "System.StringComparer.OrdinalIgnoreCase"
      /// </summary>
      public string? Comparer { get; set; }
   }
}
