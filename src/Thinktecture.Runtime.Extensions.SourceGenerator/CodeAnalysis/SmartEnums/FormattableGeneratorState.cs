namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct FormattableGeneratorState : IEquatable<FormattableGeneratorState>
{
   public ITypeInformation Type { get; }
   public IMemberInformation KeyMember { get; }
   public bool SkipIFormattable { get; }
   public bool IsKeyMemberFormattable { get; }

   public FormattableGeneratorState(
      ITypeInformation type,
      IMemberInformation keyMember,
      bool skipIFormattable,
      bool isKeyMemberFormattable)
   {
      Type = type;
      KeyMember = keyMember;
      SkipIFormattable = skipIFormattable;
      IsKeyMemberFormattable = isKeyMemberFormattable;
   }

   public bool Equals(FormattableGeneratorState other)
   {
      return TypeInformationComparer.Instance.Equals(Type, other.Type)
             && MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)
             && SkipIFormattable == other.SkipIFormattable
             && IsKeyMemberFormattable == other.IsKeyMemberFormattable;
   }

   public override bool Equals(object? obj)
   {
      return obj is FormattableGeneratorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeInformationComparer.Instance.GetHashCode(Type);
         hashCode = (hashCode * 397) ^ MemberInformationComparer.Instance.GetHashCode(KeyMember);
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsKeyMemberFormattable.GetHashCode();

         return hashCode;
      }
   }
}
