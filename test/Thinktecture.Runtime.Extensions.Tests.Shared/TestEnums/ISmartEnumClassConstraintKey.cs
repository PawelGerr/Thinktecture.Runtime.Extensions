namespace Thinktecture.Runtime.Tests.TestEnums;

public interface ISmartEnumClassConstraintKey<T>
   where T : class, ISmartEnumClassConstraintKey<T>
{
   static abstract T Item1 { get; }
   static abstract T Item2 { get; }
}
