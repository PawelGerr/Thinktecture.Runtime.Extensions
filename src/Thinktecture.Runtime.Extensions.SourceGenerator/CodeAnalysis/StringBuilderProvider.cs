using System.Text;

namespace Thinktecture.CodeAnalysis;

public class StringBuilderProvider
{
   private StringBuilder? _stringBuilder;

   public StringBuilder GetStringBuilder(int initialCapacity)
   {
      if (_stringBuilder == null)
      {
         _stringBuilder = new StringBuilder(initialCapacity);
      }
      else
      {
         _stringBuilder.Clear();
      }

      return _stringBuilder;
   }
}
