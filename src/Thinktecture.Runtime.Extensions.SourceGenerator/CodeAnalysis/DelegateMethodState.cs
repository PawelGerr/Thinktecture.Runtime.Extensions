namespace Thinktecture.CodeAnalysis;

public sealed class DelegateMethodState : IEquatable<DelegateMethodState>, IHashCodeComputable
{
   public Accessibility Accessibility { get; }
   public string MethodName { get; }
   public string? ReturnType { get; }
   public IReadOnlyList<ParameterState> Parameters { get; }
   public string ArgumentName { get; }

   public DelegateMethodState(
      Accessibility accessibility,
      string methodName,
      string? returnType,
      IReadOnlyList<ParameterState> parameters)
   {
      Accessibility = accessibility;
      MethodName = methodName;
      ReturnType = returnType;
      Parameters = parameters;
      ArgumentName = methodName.MakeArgumentName();
   }

   public bool Equals(DelegateMethodState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Accessibility == other.Accessibility
             && MethodName == other.MethodName
             && ReturnType == other.ReturnType
             && Parameters.SequenceEqual(other.Parameters);
   }

   public override bool Equals(object? obj)
   {
      return obj is DelegateMethodState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = (int)Accessibility;
         hashCode = (hashCode * 397) ^ MethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ ReturnType?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ Parameters.ComputeHashCode();
         return hashCode;
      }
   }

   public sealed class ParameterState : IEquatable<ParameterState>, IHashCodeComputable
   {
      public string Name { get; }
      public string Type { get; }
      public RefKind RefKind { get; }

      public ParameterState(
         string name,
         string type,
         RefKind refKind)
      {
         Name = name;
         Type = type;
         RefKind = refKind;
      }

      public override bool Equals(object? obj)
      {
         return obj is ParameterState other && Equals(other);
      }

      public bool Equals(ParameterState? other)
      {
         if (other is null)
            return false;

         if (ReferenceEquals(this, other))
            return true;

         return Name == other.Name
                && Type == other.Type
                && RefKind == other.RefKind;
      }

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)RefKind;
            return hashCode;
         }
      }
   }
}
