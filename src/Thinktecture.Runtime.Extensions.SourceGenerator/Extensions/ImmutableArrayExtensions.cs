using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class ImmutableArrayExtensions
{
   public static IReadOnlyList<T> GetDistinctInnerStates<T>(
      this ImmutableArray<SourceGenState<T>> states,
      SourceProductionContext context)
      where T : IEquatable<T>
   {
      var innerStates = new List<T>();

      foreach (var state in states)
      {
         if (state.Exception is not null)
         {
            context.ReportException(state.Exception);
            continue;
         }

         var innerState = state.State;

         if (innerState is null)
            continue;

         if (!innerStates.Contains(innerState))
            innerStates.Add(innerState);
      }

      return innerStates;
   }
}
