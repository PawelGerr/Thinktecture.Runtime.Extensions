using System.Diagnostics.CodeAnalysis;
using Serilog.Core;
using Serilog.Events;
using Thinktecture.Internal;

namespace Thinktecture;

internal sealed class ThinktectureDestructuringPolicy : IDestructuringPolicy
{
   private readonly TypesToRenderAsString _renderAsString;

   public ThinktectureDestructuringPolicy(TypesToRenderAsString renderAsString)
   {
      _renderAsString = renderAsString;
   }

   public bool TryDestructure(
      object value,
      ILogEventPropertyValueFactory propertyValueFactory,
      [NotNullWhen(true)] out LogEventPropertyValue? result)
   {
      // Find(...) returns null fast for non-Thinktecture types (IMetadataOwner check).
      var metadata = MetadataLookup.Find(value.GetType());

      if (metadata is null)
      {
         result = null;
         return false;
      }

      // Exhaustive Switch over the Metadata union: a new Metadata kind makes this stop compiling,
      // forcing a deliberate decision instead of silently declining. State is passed explicitly
      // (no captured closures) so this stays allocation-free on the per-property logging path.
      // Declined kinds return null, which maps to "decline" (return false) below.
      var state = new DestructureState(value, propertyValueFactory, _renderAsString);

      result = metadata.Switch<DestructureState, LogEventPropertyValue?>(
         state,
         adHocUnion: static (s, union) => (s.RenderAsString & TypesToRenderAsString.AdHocUnions) != 0
                                             ? new ScalarValue(s.Value)
                                             : s.Factory.CreatePropertyValue(union.GetValue(s.Value), destructureObjects: true),
         keyedSmartEnum: static (s, smartEnum) => (s.RenderAsString & TypesToRenderAsString.SmartEnums) != 0
                                                     ? new ScalarValue(s.Value)
                                                     : s.Factory.CreatePropertyValue(smartEnum.GetKey(s.Value), destructureObjects: true),
         keyedValueObject: static (s, valueObject) => (s.RenderAsString & TypesToRenderAsString.ValueObjects) != 0
                                                         ? new ScalarValue(s.Value)
                                                         : s.Factory.CreatePropertyValue(valueObject.GetKey(s.Value), destructureObjects: true),
         keylessSmartEnum: static (_, _) => null,
         complexValueObject: static (_, _) => null,
         regularUnion: static (_, _) => null);

      return result is not null;
   }

   private readonly struct DestructureState(
      object value,
      ILogEventPropertyValueFactory factory,
      TypesToRenderAsString renderAsString)
   {
      public object Value { get; } = value;
      public ILogEventPropertyValueFactory Factory { get; } = factory;
      public TypesToRenderAsString RenderAsString { get; } = renderAsString;
   }
}
