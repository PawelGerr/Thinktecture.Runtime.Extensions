using System;

namespace Thinktecture.SmartEnums;

[SmartEnum]
public partial class MoneyRoundingStrategy
{
   public static readonly MoneyRoundingStrategy Default = new(d => decimal.Round(d, 2));
   public static readonly MoneyRoundingStrategy Up = new(d => decimal.Round(d, 2, MidpointRounding.ToPositiveInfinity));
   public static readonly MoneyRoundingStrategy Down = new(d => decimal.Round(d, 2, MidpointRounding.ToNegativeInfinity));

   [UseDelegateFromConstructor]
   public partial decimal Round(decimal value);
}
