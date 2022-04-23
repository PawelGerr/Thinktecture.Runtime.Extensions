namespace Thinktecture.CodeAnalysis.SmartEnums;

public interface IBaseEnumState : IEquatable<IBaseEnumState>
{
   bool IsSameAssembly { get; }
   string TypeFullyQualified { get; }
   string TypeMinimallyQualified { get; }
   string? NullableQuestionMark { get; }
   IReadOnlyList<IMemberState> Items { get; }
   IReadOnlyList<IMemberState> ConstructorArguments { get; }
   EnumSettings Settings { get; }

   bool HasJsonConverterFactory { get; }
   bool HasNewtonsoftJsonConverter { get; }
   bool HasMessagePackFormatter { get; }
}
