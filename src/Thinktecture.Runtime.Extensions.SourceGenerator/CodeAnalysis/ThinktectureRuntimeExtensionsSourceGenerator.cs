using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   /// <summary>
   /// Source generator for enum-like class and value object.
   /// </summary>
   [Generator]
   public class ThinktectureRuntimeExtensionsSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      public ThinktectureRuntimeExtensionsSourceGenerator()
         : base(null)
      {
      }

      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         return new EnumSourceGenerator(state).Generate();
      }

      /// <inheritdoc />
      protected override string GenerateValueObject(ValueObjectSourceGeneratorState state)
      {
         return new ValueObjectSourceGenerator(state).Generate();
      }
   }
}
