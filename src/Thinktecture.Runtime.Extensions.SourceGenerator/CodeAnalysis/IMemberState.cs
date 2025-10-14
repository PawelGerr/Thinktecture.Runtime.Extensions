namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>, IMemberInformation, IHashCodeComputable
{
   string ArgumentName { get; }
}
