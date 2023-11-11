using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SerializationFrameworksExtensions
{
   public static bool Has(this SerializationFrameworks value, SerializationFrameworks flag)
   {
      return (value & flag) == flag;
   }
}
