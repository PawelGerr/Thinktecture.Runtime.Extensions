using Microsoft.CodeAnalysis;

namespace Thinktecture;

public static class AccessibilityExtensions
{
   public static bool IsAtLeastProtected(this Accessibility accessibility)
   {
      return accessibility is Accessibility.Protected or Accessibility.ProtectedAndFriend or Accessibility.ProtectedAndInternal or Accessibility.Public;
   }
}
