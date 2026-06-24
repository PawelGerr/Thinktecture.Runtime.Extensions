using Serilog;
using Serilog.Configuration;

namespace Thinktecture;

/// <summary>
/// Extension methods for <see cref="LoggerDestructuringConfiguration"/>.
/// </summary>
public static class LoggerDestructuringConfigurationExtensions
{
   /// <summary>
   /// Registers a destructuring policy that unwraps Thinktecture Smart Enums, Value Objects and Ad-hoc Unions
   /// to their underlying payload when they are destructured by Serilog.
   /// </summary>
   /// <param name="configuration">The Serilog destructuring configuration.</param>
   /// <param name="renderAsString">
   /// Selects which type families are rendered as a flat string via <c>ToString()</c> instead of being unwrapped.
   /// Defaults to <see cref="TypesToRenderAsString.None"/> (all supported families are unwrapped).
   /// </param>
   /// <returns>The logger configuration, to enable method chaining.</returns>
   public static LoggerConfiguration UsingThinktectureRuntimeExtensions(
      this LoggerDestructuringConfiguration configuration,
      TypesToRenderAsString renderAsString = TypesToRenderAsString.None)
   {
      return configuration.With(new ThinktectureDestructuringPolicy(renderAsString));
   }
}
