namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
public partial class SmartEnum_ArrayTypeStaticMember
{
   private static readonly SmartEnum_ArrayTypeStaticMember[] InternalMember = [];

   public static readonly SmartEnum_ArrayTypeStaticMember Item = new(1);
}
