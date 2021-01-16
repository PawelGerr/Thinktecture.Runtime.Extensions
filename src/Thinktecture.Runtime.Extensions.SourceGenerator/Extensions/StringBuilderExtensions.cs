using System.Text;
using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class StringBuilderExtensions
   {
      public static void GenerateStructLayoutAttributeIfRequired(this StringBuilder sb, ITypeSymbol type)
      {
         if (type.IsValueType && !type.HasStructLayoutAttribute())
         {
            sb.Append($@"
   [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]");
         }
      }
   }
}
