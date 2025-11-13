using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<DateTime>]
public partial class SmartEnum_DateTimeBased
{
   public static readonly SmartEnum_DateTimeBased Item1 = new(new DateTime(2024, 1, 1));
   public static readonly SmartEnum_DateTimeBased Item2 = new(new DateTime(2024, 12, 31));
}
