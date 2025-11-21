namespace Thinktecture.CodeAnalysis;

public sealed class GeneratorOptions : IEquatable<GeneratorOptions>
{
   public bool CounterEnabled { get; }
   public bool GenerateJetbrainsAnnotations { get; }
   public LoggingOptions? Logging { get; }

   public GeneratorOptions(bool counterEnabled,
                           bool generateJetbrainsAnnotations,
                           LoggingOptions? logging)
   {
      CounterEnabled = counterEnabled;
      GenerateJetbrainsAnnotations = generateJetbrainsAnnotations;
      Logging = logging;
   }

   public bool Equals(GeneratorOptions? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return CounterEnabled == other.CounterEnabled
             && GenerateJetbrainsAnnotations == other.GenerateJetbrainsAnnotations
             && Logging.NullableStructEquals(other.Logging);
   }

   public override bool Equals(object? obj)
   {
      return obj is GeneratorOptions other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (CounterEnabled.GetHashCode() * 397)
                ^ (GenerateJetbrainsAnnotations.GetHashCode() * 397)
                ^ Logging.GetHashCode();
      }
   }
}
