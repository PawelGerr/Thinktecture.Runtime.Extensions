namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<TypeParamRef1>]
public partial class SmartEnum_GenericKeyBasedClassConstraint<T>
   where T : class, ISmartEnumClassConstraintKey<T>
{
   public static readonly SmartEnum_GenericKeyBasedClassConstraint<T> Item1 = new(T.Item1);
   public static readonly SmartEnum_GenericKeyBasedClassConstraint<T> Item2 = new(T.Item2);
}
