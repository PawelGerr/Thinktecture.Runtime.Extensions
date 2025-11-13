namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.None)]
public partial class SmartEnum_without_SerializationFrameworks
{
   public static readonly SmartEnum_without_SerializationFrameworks Item1 = new("Item1");
   public static readonly SmartEnum_without_SerializationFrameworks Item2 = new("Item2");
}
