namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>(SkipToString = true)]
public partial class SmartEnum_NameDuplicates
{
   public static readonly SmartEnum_NameDuplicates Item1 = new(1, "item");
   public static readonly SmartEnum_NameDuplicates Item2 = new(2, "item");

   public string Name { get; }

   public override string ToString()
   {
      return Name;
   }
}
