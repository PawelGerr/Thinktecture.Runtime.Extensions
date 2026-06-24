using System;
using Serilog;
using Thinktecture.SmartEnums;
using Thinktecture.Unions;
using Thinktecture.ValueObjects;

namespace Thinktecture.Logging;

// Ad-hoc union whose value is a KEYED value object. When logged, the policy unwraps the union to its
// value (the Amount), then re-enters the destructuring pipeline for that value - where the policy runs
// AGAIN and composes through to the Amount's key. The inner value is handled by OUR policy, not reflection.
[Union<Amount, string>]
public partial class AmountOrText;

// Ad-hoc union whose value is a COMPLEX value object. The union unwraps to the Boundary, the pipeline
// re-enters, the policy DECLINES the complex value object, and Serilog destructures it structurally
// (via reflection). This is what `destructureObjects: true` buys us: each inner value is routed correctly
// even though the wrapper does not know the inner type.
[Union<Boundary, int>]
public partial class BoundaryOrNumber;

public static class SerilogDestructuringDemos
{
   public static void Demo()
   {
      Console.WriteLine();
      Console.WriteLine("======================================================================");
      Console.WriteLine(" Serilog destructuring demo (real Serilog + console sink, JSON render)");
      Console.WriteLine("======================================================================");

      var amount = Amount.Create(42.50m);
      var boundary = Boundary.Create(1m, 10m);

      // ":j" renders the logged properties as JSON, so a scalar ("42.50") is visually distinct from a
      // reflected structure ("{ Lower: 1, Upper: 10 }").
      using (var logger = new LoggerConfiguration()
                          .WriteTo.Console(outputTemplate: "  {Message:j}{NewLine}")
                          .Destructure.UsingThinktectureRuntimeExtensions()
                          .CreateLogger())
      {
         Console.WriteLine();
         Console.WriteLine("--- default: unwrap to the underlying payload ---");
         logger.Information("keyed smart enum            -> {@Value}", ProductType.Groceries);
         logger.Information("keyed value object          -> {@Value}", amount);
         logger.Information("ad-hoc union of scalars     -> {@Value}", (TextOrNumber)42);
         logger.Information("union -> keyed value object -> {@Value}", (AmountOrText)amount);
         logger.Information("union -> complex value obj  -> {@Value}", (BoundaryOrNumber)boundary);

         Console.WriteLine();
         Console.WriteLine("--- declined by the policy -> Serilog's default (reflection) destructuring ---");
         logger.Information("complex value object        -> {@Value}", boundary);
      }

      using (var asString = new LoggerConfiguration()
                            .WriteTo.Console(outputTemplate: "  {Message:j}{NewLine}")
                            .Destructure.UsingThinktectureRuntimeExtensions(TypesToRenderAsString.All)
                            .CreateLogger())
      {
         Console.WriteLine();
         Console.WriteLine("--- TypesToRenderAsString.All: render via ToString() instead of unwrapping ---");
         asString.Information("keyed smart enum            -> {@Value}", ProductType.Groceries);
         asString.Information("keyed value object          -> {@Value}", amount);
         asString.Information("ad-hoc union of scalars     -> {@Value}", (TextOrNumber)42);
      }

      Console.WriteLine();
   }
}
