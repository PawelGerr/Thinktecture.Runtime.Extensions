using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class KeyMemberSettingsExtensions
{
   public static KeyMemberState CreateKeyMember(this IKeyMemberSettings settings, ITypedMemberState keyMemberState)
   {
      return new KeyMemberState(keyMemberState,
                                settings.KeyMemberAccessModifier,
                                settings.KeyMemberKind,
                                settings.KeyMemberName,
                                ArgumentName.Create(settings.KeyMemberName));
   }
}
