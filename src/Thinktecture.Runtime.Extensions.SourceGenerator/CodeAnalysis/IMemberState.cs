namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>, IMemberInformation, IHashCodeComputable
{
   ArgumentName ArgumentName { get; }
}
