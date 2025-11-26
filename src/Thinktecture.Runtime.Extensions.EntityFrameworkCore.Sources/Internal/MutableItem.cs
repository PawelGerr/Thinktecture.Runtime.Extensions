using Microsoft.EntityFrameworkCore.Metadata;

namespace Thinktecture.Internal;

internal readonly struct MutableItem
{
   private readonly IMutableProperty? _property;
   private readonly IMutableElementType? _elementType;

   public Type Type { get; }

   public MutableItem(Type type, IMutableProperty property)
      : this(type, property, null)
   {
   }

   public MutableItem(Type type, IMutableElementType elementType)
      : this(type, null, elementType)
   {
   }

   private MutableItem(Type type, IMutableProperty? property, IMutableElementType? elementType)
   {
      Type = type;
      _property = property;
      _elementType = elementType;
   }

   public int? GetMaxLength()
   {
      return _property?.GetMaxLength() ?? _elementType?.GetMaxLength();
   }

   public void SetMaxLength(int? maxLength)
   {
      _property?.SetMaxLength(maxLength);
      _elementType?.SetMaxLength(maxLength);
   }
}
