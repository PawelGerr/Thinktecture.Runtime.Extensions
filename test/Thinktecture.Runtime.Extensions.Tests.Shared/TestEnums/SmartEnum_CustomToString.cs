namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(SkipToString = true)]
public partial class SmartEnum_CustomToString
{
   public static readonly SmartEnum_CustomToString Item1 = new(1, "item1");
   public static readonly SmartEnum_CustomToString Item2 = new(2, "item 2");

   public string Name { get; }

   public override string ToString()
   {
      return Name;
   }
}
