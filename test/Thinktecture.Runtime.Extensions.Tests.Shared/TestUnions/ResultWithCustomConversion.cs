namespace Thinktecture.Runtime.Tests.TestUnions;

[Union(SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
       SkipImplicitConversionFromValue = true)]
public partial record ResultWithCustomConversion<T>
{
   public sealed record Success(T Value) : ResultWithCustomConversion<T>;

   public sealed record Failure(string Error) : ResultWithCustomConversion<T>;

   public static implicit operator ResultWithCustomConversion<T>(T value) => new Success(value);
   public static implicit operator ResultWithCustomConversion<T>(string error) => new Failure(error);
}
