using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class InterfaceCodeGeneratorFactory<T, TType>(IInterfaceCodeGenerator<T> interfaceCodeGenerator)
   : ICodeGeneratorFactory<T>
   where T : ITypeInformationProvider<TType>, IHasGenerics
   where TType : ITypeFullyQualified, INamespaceAndName, ITypeKindInformation
{
   public string CodeGeneratorName => interfaceCodeGenerator.CodeGeneratorName;

   public CodeGeneratorBase Create(T state, StringBuilder stringBuilder)
   {
      return new InterfaceCodeGenerator<T, TType>(interfaceCodeGenerator, state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<T> other)
   {
      return ReferenceEquals(this, other);
   }
}

public static class InterfaceCodeGeneratorFactory
{
   private static readonly ICodeGeneratorFactory<SpanParsableGeneratorState> _spanParsableForValueObject = new InterfaceCodeGeneratorFactory<SpanParsableGeneratorState, IParsableTypeInformation>(SpanParsableCodeGenerator.ForValueObject);
   private static readonly ICodeGeneratorFactory<SpanParsableGeneratorState> _spanParsableForEnum = new InterfaceCodeGeneratorFactory<SpanParsableGeneratorState, IParsableTypeInformation>(SpanParsableCodeGenerator.ForEnum);
   private static readonly ICodeGeneratorFactory<InterfaceCodeGeneratorState> _comparable = new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(ComparableCodeGenerator.Default);

   public static readonly ICodeGeneratorFactory<ParsableGeneratorState> Parsable = new InterfaceCodeGeneratorFactory<ParsableGeneratorState, IParsableTypeInformation>(ParsableCodeGenerator.Instance);
   public static readonly ICodeGeneratorFactory<InterfaceCodeGeneratorState> Formattable = new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(FormattableCodeGenerator.Instance);

   public static ICodeGeneratorFactory<InterfaceCodeGeneratorState> Comparable(string? comparerAccessor)
   {
      return String.IsNullOrWhiteSpace(comparerAccessor)
                ? _comparable
                : new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(new ComparableCodeGenerator(comparerAccessor));
   }

   public static ICodeGeneratorFactory<SpanParsableGeneratorState> SpanParsable(bool forEnum)
   {
      return forEnum
                ? _spanParsableForEnum
                : _spanParsableForValueObject;
   }

   public static bool EqualityComparison(
      OperatorsGeneration operatorsGeneration,
      ComparerInfo? equalityComparer,
      [MaybeNullWhen(false)] out ICodeGeneratorFactory<EqualityComparisonOperatorsGeneratorState> generatorFactory)
   {
      if (EqualityComparisonOperatorsCodeGenerator.TryGet(operatorsGeneration, equalityComparer, out var generator))
      {
         generatorFactory = new InterfaceCodeGeneratorFactory<EqualityComparisonOperatorsGeneratorState, ITypeInformation>(generator);
         return true;
      }

      generatorFactory = null;
      return false;
   }

   public static ICodeGeneratorFactory<InterfaceCodeGeneratorState> Create(IInterfaceCodeGenerator codeGenerator)
   {
      return new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(codeGenerator);
   }
}
