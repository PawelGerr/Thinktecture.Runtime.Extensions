namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_ReservedIdentifier
{
   public static readonly SmartEnum_ReservedIdentifier Operator = new(1);
   public static readonly SmartEnum_ReservedIdentifier True = new(2);
}
