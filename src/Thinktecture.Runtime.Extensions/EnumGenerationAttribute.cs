using System;

namespace Thinktecture
{
   /// <summary>
   /// Settings to be used by source generator.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class)]
   public class EnumGenerationAttribute : Attribute
   {
      /// <summary>
      /// The static member name containing the key equality comparer.
      /// </summary>
      public string? KeyComparerProvidingMember { get; set; }
   }
}
