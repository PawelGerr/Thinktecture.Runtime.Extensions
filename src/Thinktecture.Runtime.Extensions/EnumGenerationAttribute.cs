using System;

namespace Thinktecture
{
   /// <summary>
   /// Settings to be used by source generator.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public class EnumGenerationAttribute : Attribute
   {
      /// <summary>
      /// The static member name containing the key equality comparer.
      /// </summary>
      public string? KeyComparerProvidingMember { get; set; }

      private string? _keyPropertyName;

      /// <summary>
      /// The name of the key property. Default name is 'Key'.
      /// </summary>
      public string KeyPropertyName
      {
         get => _keyPropertyName ?? "Key";
         set => _keyPropertyName = value;
      }
   }
}
