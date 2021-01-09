using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   /// <summary>
   /// Source generator for enum-like class and value type.
   /// </summary>
   [Generator]
   public class ThinktectureRuntimeExtensionsSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         return new EnumSourceGenerator(state).Generate();
      }

      /// <inheritdoc />
      protected override string GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         return new ValueTypeSourceGenerator(state).Generate();
      }
   }
}
