using System.Numerics;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TypeParamRef1>]
public partial class SmartEnum_GenericKeyBasedStructConstraint<T>
   where T : struct, INumber<T>
{
   public static readonly SmartEnum_GenericKeyBasedStructConstraint<T> Item1 = new(T.One);
   public static readonly SmartEnum_GenericKeyBasedStructConstraint<T> Item2 = new(T.One + T.One);
}
