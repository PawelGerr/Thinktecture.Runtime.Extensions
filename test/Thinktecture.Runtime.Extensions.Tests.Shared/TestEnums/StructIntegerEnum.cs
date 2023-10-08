namespace Thinktecture.Runtime.Tests.TestEnums;

[SmartEnum<int>(IsValidatable = true)]
public readonly partial struct StructIntegerEnum
{
   public static readonly StructIntegerEnum Item1 = new(1, 42, 100);
   public static readonly StructIntegerEnum Item2 = new(2, 43, 200);

   // ReSharper disable once UnusedMember.Global
   public int Property1 => Field;

   // ReSharper disable once UnusedMember.Global
   public int Property2
   {
      // ReSharper disable once ArrangeAccessorOwnerBody
      get { return Field; }
   }

   public int Property3 { get; }
   public readonly int Field;
}
