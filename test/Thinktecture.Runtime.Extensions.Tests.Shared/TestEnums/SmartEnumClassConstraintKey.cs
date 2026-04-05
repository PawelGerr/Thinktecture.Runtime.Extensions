namespace Thinktecture.Runtime.Tests.TestEnums;

public record SmartEnumClassConstraintKey(string Name) : ISmartEnumClassConstraintKey<SmartEnumClassConstraintKey>
{
   public static SmartEnumClassConstraintKey Item1 { get; } = new(nameof(Item1));
   public static SmartEnumClassConstraintKey Item2 { get; } = new(nameof(Item2));
}
