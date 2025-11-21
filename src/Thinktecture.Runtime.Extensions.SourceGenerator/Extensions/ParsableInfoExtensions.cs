using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ParsableInfoExtensions
{
   public static bool MustGenerateIParsable(this IParsableState state)
   {
      return !state.SkipIParsable &&
             (state.KeyMember?.IsString() == true || state.KeyMember?.IsParsable == true || state.HasStringBasedValidateMethod);
   }
}
