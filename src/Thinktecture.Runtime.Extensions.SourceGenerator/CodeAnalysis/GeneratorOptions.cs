namespace Thinktecture.CodeAnalysis;

public sealed class GeneratorOptions : IEquatable<GeneratorOptions>
{
   public bool CounterEnabled { get; }
   public LoggingOptions? Logging { get; }

   public GeneratorOptions(
      bool counterEnabled,
      LoggingOptions? logging)
   {
      CounterEnabled = counterEnabled;
      Logging = logging;
   }

   public bool Equals(GeneratorOptions? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return CounterEnabled == other.CounterEnabled
             && Logging.EqualsTo(other.Logging);
   }

   public override bool Equals(object? obj)
   {
      return obj is GeneratorOptions other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (CounterEnabled.GetHashCode() * 397) ^ Logging.GetHashCode();
      }
   }
}
