using System.Diagnostics.CodeAnalysis;
using System.Text;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture.CodeAnalysis;

public class InterfaceCodeGeneratorFactory<T, TType> : ICodeGeneratorFactory<T>
   where T : ITypeInformationProvider<TType>
   where TType : ITypeFullyQualified, INamespaceAndName, ITypeKindInformation
{
   private readonly IInterfaceCodeGenerator<T> _interfaceCodeGenerator;

   public string CodeGeneratorName => _interfaceCodeGenerator.CodeGeneratorName;

   public InterfaceCodeGeneratorFactory(IInterfaceCodeGenerator<T> interfaceCodeGenerator)
   {
      _interfaceCodeGenerator = interfaceCodeGenerator;
   }

   public CodeGeneratorBase Create(T state, StringBuilder stringBuilder)
   {
      return new InterfaceCodeGenerator<T, TType>(_interfaceCodeGenerator, state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<T> other)
   {
      return ReferenceEquals(this, other);
   }
}

public static class InterfaceCodeGeneratorFactory
{
   private static readonly ICodeGeneratorFactory<ParsableGeneratorState> _parsableForValueObject = new InterfaceCodeGeneratorFactory<ParsableGeneratorState, IParsableTypeInformation>(ParsableCodeGenerator.ForValueObject);
   private static readonly ICodeGeneratorFactory<ParsableGeneratorState> _parsableForEnum = new InterfaceCodeGeneratorFactory<ParsableGeneratorState, IParsableTypeInformation>(ParsableCodeGenerator.ForEnum);
   private static readonly ICodeGeneratorFactory<ParsableGeneratorState> _parsableForValidatableEnum = new InterfaceCodeGeneratorFactory<ParsableGeneratorState, IParsableTypeInformation>(ParsableCodeGenerator.ForValidatableEnum);
   private static readonly ICodeGeneratorFactory<InterfaceCodeGeneratorState> _comparable = new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(ComparableCodeGenerator.Default);

   public static readonly ICodeGeneratorFactory<InterfaceCodeGeneratorState> Formattable = new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(FormattableCodeGenerator.Instance);

   public static ICodeGeneratorFactory<InterfaceCodeGeneratorState> Comparable(string? comparerAccessor)
   {
      return String.IsNullOrWhiteSpace(comparerAccessor) ? _comparable : new InterfaceCodeGeneratorFactory<InterfaceCodeGeneratorState, ITypeInformation>(new ComparableCodeGenerator(comparerAccessor));
   }

   public static ICodeGeneratorFactory<ParsableGeneratorState> Parsable(
      bool forEnum,
      bool forValidatableEnum)
   {
      return forEnum
                ? forValidatableEnum ? _parsableForValidatableEnum : _parsableForEnum
                : _parsableForValueObject;
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
