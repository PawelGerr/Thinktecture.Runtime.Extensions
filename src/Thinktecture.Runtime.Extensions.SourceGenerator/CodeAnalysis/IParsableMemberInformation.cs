namespace Thinktecture.CodeAnalysis;

public interface IParsableMemberInformation : IMemberInformation
{
   bool IsParsable { get; }
   bool IsSpanParsable { get; }
}
