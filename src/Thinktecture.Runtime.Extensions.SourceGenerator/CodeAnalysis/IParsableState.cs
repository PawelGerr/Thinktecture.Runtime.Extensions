namespace Thinktecture.CodeAnalysis;

public interface IParsableState
{
   IParsableMemberInformation? KeyMember { get; }
   bool SkipIParsable { get; }
   bool HasStringBasedValidateMethod { get; }
}
