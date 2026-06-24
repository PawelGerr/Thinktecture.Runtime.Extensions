namespace Thinktecture.Runtime.Tests.TestTypes;

[SmartEnum<int>]
public partial class OrderStatus
{
   public static readonly OrderStatus Pending = new(1);
   public static readonly OrderStatus Shipped = new(2);
}
