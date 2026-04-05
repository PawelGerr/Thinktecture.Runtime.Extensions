using System.Numerics;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TypeParamRef1>]
public partial class SmartEnum_GenericKeyBasedUnconstraint<T>
   where T : INumber<T>
{
   public static readonly SmartEnum_GenericKeyBasedUnconstraint<T> Item1 = new(T.One);
   public static readonly SmartEnum_GenericKeyBasedUnconstraint<T> Item2 = new(T.One + T.One);
}
