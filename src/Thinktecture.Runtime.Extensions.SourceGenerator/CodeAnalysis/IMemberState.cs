namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IMemberInformation, IHashCodeComputable
{
   ArgumentName ArgumentName { get; }
}
