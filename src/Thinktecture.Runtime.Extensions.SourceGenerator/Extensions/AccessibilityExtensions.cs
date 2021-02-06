using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class AccessibilityExtensions
   {
      public static bool IsAtLeastProtected(this Accessibility accessibility)
      {
         return accessibility == Accessibility.Protected ||
                accessibility == Accessibility.ProtectedAndFriend ||
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                accessibility == Accessibility.ProtectedAndInternal ||
                accessibility == Accessibility.Public;
      }
   }
}
