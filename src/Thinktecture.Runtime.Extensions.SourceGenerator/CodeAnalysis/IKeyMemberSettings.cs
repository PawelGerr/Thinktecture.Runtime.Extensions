namespace Thinktecture.CodeAnalysis;

public interface IKeyMemberSettings
{
   ValueObjectAccessModifier KeyMemberAccessModifier { get; }
   ValueObjectMemberKind KeyMemberKind { get; }
   string KeyMemberName { get; }
   bool SkipKeyMember { get; }
}
