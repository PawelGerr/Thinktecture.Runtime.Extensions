namespace Thinktecture.CodeAnalysis;

public interface IBaseEnumState<TExtension> : IEquatable<IBaseEnumState<TExtension>>
   where TExtension : IEquatable<TExtension>
{
   bool IsSameAssembly { get; }
   string TypeFullyQualified { get; }
   string TypeMinimallyQualified { get; }
   string? NullableQuestionMark { get; }
   IReadOnlyList<IMemberState> Items { get; }
   IReadOnlyList<IMemberState> ConstructorArguments { get; }
   EnumSettings Settings { get; }

   TExtension Extension { get; }
}
