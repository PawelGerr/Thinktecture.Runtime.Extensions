namespace Thinktecture.CodeAnalysis;

public interface IKeyMemberSettings
{
   AccessModifier KeyMemberAccessModifier { get; }
   MemberKind KeyMemberKind { get; }
   string KeyMemberName { get; }
}
