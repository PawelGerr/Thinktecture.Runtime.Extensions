using System;

namespace Thinktecture
{
   /// <summary>
   /// Marks the type as a value type.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public class ValueTypeAttribute : Attribute
   {
      /// <summary>
      /// Indication whether the methods "Create" and "TryCreate" should be generated or not.
      /// </summary>
      public bool SkipFactoryMethods { get; set; }
   }
}
