using System;

namespace Thinktecture.Runtime.Tests.TestUnions;

[Union]
public partial record TestUnionWithGenerics<T>
   where T : IFormattable
{
   public record First(T Value) : TestUnionWithGenerics<T>;

   public abstract record Second<TOther>(TOther Value) : TestUnionWithGenerics<TOther>
      where TOther : T, IFormattable;

   public record Third(T Value) : Second<T>(Value);
}
