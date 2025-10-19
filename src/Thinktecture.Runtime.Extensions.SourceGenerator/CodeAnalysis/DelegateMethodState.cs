namespace Thinktecture.CodeAnalysis;

public sealed class DelegateMethodState : IEquatable<DelegateMethodState>, IHashCodeComputable
{
   public Accessibility Accessibility { get; }
   public string MethodName { get; }
   public string? ReturnType { get; }
   public IReadOnlyList<ParameterState> Parameters { get; }
   public ArgumentName ArgumentName { get; }
   public string? DelegateName { get; }

   public DelegateMethodState(
      Accessibility accessibility,
      string methodName,
      string? returnType,
      IReadOnlyList<ParameterState> parameters,
      string? delegateName)
   {
      Accessibility = accessibility;
      MethodName = methodName;
      ReturnType = returnType;
      Parameters = parameters;
      DelegateName = delegateName;
      ArgumentName = ArgumentName.Create(delegateName ?? methodName);
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
             && Parameters.SequenceEqual(other.Parameters)
             && DelegateName == other.DelegateName;
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
         hashCode = (hashCode * 397) ^ (ReturnType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Parameters.ComputeHashCode();
         hashCode = (hashCode * 397) ^ (DelegateName?.GetHashCode() ?? 0);
         return hashCode;
      }
   }
}
