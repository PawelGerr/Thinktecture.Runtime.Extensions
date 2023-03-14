namespace Thinktecture.CodeAnalysis;

public sealed class GeneratorOptions : IEquatable<GeneratorOptions>
{
   public bool CounterEnabled { get; }

   public GeneratorOptions(bool counterEnabled)
   {
      CounterEnabled = counterEnabled;
   }

   public bool Equals(GeneratorOptions? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return CounterEnabled == other.CounterEnabled;
   }

   public override bool Equals(object? obj)
   {
      return obj is GeneratorOptions other && Equals(other);
   }

   public override int GetHashCode()
   {
      return CounterEnabled.GetHashCode();
   }
}
