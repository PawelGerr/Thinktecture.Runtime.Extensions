namespace Thinktecture.CodeAnalysis;

public enum ValueObjectAccessModifier
{
   Private = 1 << 0,
   Protected = 1 << 1,
   Internal = 1 << 2,
   Public = 1 << 3,
   PrivateProtected = Private | Protected,
   ProtectedInternal = Protected | Internal
}
