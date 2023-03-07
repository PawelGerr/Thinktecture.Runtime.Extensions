using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberState : IEquatable<TypedMemberState>, ITypedMemberState
{
#pragma warning disable RS1024 // we don't need SymbolComparer because we compare SpecialType and NullableAnnotation only.
   private static readonly ConcurrentDictionary<ITypeSymbol, ITypedMemberState?> _typedMemberStates = new(SpecialTypeAndNullableAnnotationComparer.Instance);
#pragma warning restore RS1024

   private readonly ITypeSymbol _type;

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => _type.IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => _type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated } ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }

   public NullableAnnotation NullableAnnotation => _type.NullableAnnotation;
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsReferenceType => _type.IsReferenceType;
   public bool IsNullableStruct => _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public bool IsReferenceTypeOrNullableStruct => _type.IsReferenceType || IsNullableStruct;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool HasComparisonOperators { get; }
   public bool HasAdditionOperators { get; }
   public bool HasSubtractionOperators { get; }
   public bool HasMultiplyOperators { get; }
   public bool HasDivisionOperators { get; }

   private TypedMemberState(ITypeSymbol type)
   {
      _type = type;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(_type))
         {
            IsComparable = true;
         }
         else if (@interface.IsParsableInterface(_type))
         {
            IsParsable = true;
         }
         else if (@interface.IsIAdditionOperators(_type))
         {
            HasAdditionOperators = true;
         }
         else if (@interface.IsISubtractionOperators(_type))
         {
            HasSubtractionOperators = true;
         }
         else if (@interface.IsIMultiplyOperators(_type))
         {
            HasMultiplyOperators = true;
         }
         else if (@interface.IsIDivisionOperators(_type))
         {
            HasDivisionOperators = true;
         }
         else if (@interface.IsIComparisonOperators(_type))
         {
            HasComparisonOperators = true;
         }
      }
   }

   public static ITypedMemberState GetOrCreate(ITypeSymbol type)
   {
      var cachedState = _typedMemberStates.GetOrAdd(type,
                                                    static t =>
                                                    {
                                                       switch (t.SpecialType)
                                                       {
                                                          case SpecialType.System_Boolean:
                                                          case SpecialType.System_Char:
                                                          case SpecialType.System_SByte:
                                                          case SpecialType.System_Byte:
                                                          case SpecialType.System_Int16:
                                                          case SpecialType.System_UInt16:
                                                          case SpecialType.System_Int32:
                                                          case SpecialType.System_UInt32:
                                                          case SpecialType.System_Int64:
                                                          case SpecialType.System_UInt64:
                                                          case SpecialType.System_Decimal:
                                                          case SpecialType.System_Single:
                                                          case SpecialType.System_Double:
                                                          case SpecialType.System_String:
                                                          case SpecialType.System_DateTime:
                                                             return new CachingTypedMemberState(new TypedMemberState(t));

                                                          default:
                                                             return null;
                                                       }
                                                    });

      return cachedState ?? new TypedMemberState(type);
   }

   public override bool Equals(object? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(ITypedMemberState? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(TypedMemberState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && SpecialType == other.SpecialType
             && _type.OriginalDefinition.SpecialType == other._type.OriginalDefinition.SpecialType
             && IsReferenceType == other.IsReferenceType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
             && HasComparisonOperators == other.HasComparisonOperators
             && HasAdditionOperators == other.HasAdditionOperators
             && HasSubtractionOperators == other.HasSubtractionOperators
             && HasMultiplyOperators == other.HasMultiplyOperators
             && HasDivisionOperators == other.HasDivisionOperators;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ _type.OriginalDefinition.SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasAdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasSubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasDivisionOperators.GetHashCode();

         return hashCode;
      }
   }

   // We need to check NullableAnnotation as well, because string is a reference type.
   private class SpecialTypeAndNullableAnnotationComparer : IEqualityComparer<ITypeSymbol>
   {
      public static readonly SpecialTypeAndNullableAnnotationComparer Instance = new();

      public bool Equals(ITypeSymbol x, ITypeSymbol y)
      {
         return x.SpecialType == y.SpecialType
                && x.NullableAnnotation == y.NullableAnnotation;
      }

      public int GetHashCode(ITypeSymbol obj)
      {
         return ((int)obj.SpecialType * 397) ^ (int)obj.NullableAnnotation;
      }
   }
}
