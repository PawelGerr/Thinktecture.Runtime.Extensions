using System;

namespace Thinktecture.Internal
{
   /// <summary>
   /// For internal use only.
   /// </summary>
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public class ValueTypeConstructorAttribute : Attribute
   {
      /// <summary>
      /// The names of the members of the constructor.
      /// </summary>
      public string[] Members { get; set; }

      /// <summary>
      /// Initializes new instance of <see cref="ValueTypeConstructorAttribute"/>.
      /// </summary>
      /// <param name="members">Member names of the constructor.</param>
      public ValueTypeConstructorAttribute(params string[] members)
      {
         Members = members;
      }
   }
}
