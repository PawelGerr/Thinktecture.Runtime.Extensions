namespace Thinktecture.CodeAnalysis;

public sealed class ContainingTypeState
   : IEquatable<ContainingTypeState>,
     IHasGenerics,
     IHashCodeComputable
{
   public string Name { get; }
   public bool IsReferenceType { get; }
   public bool IsRecord { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }

   public ContainingTypeState(
      string name,
      bool isReferenceType,
      bool isRecord,
      ImmutableArray<GenericTypeParameterState> genericParameters)
   {
      Name = name;
      IsReferenceType = isReferenceType;
      IsRecord = isRecord;
      GenericParameters = genericParameters;
   }

   public bool Equals(ContainingTypeState? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Name == other.Name
             && IsReferenceType == other.IsReferenceType
             && IsRecord == other.IsRecord
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is ContainingTypeState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsRecord.GetHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }
}
