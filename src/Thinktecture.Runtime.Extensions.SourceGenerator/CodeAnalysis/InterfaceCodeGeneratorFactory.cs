using System.Text;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture.CodeAnalysis;

public class InterfaceCodeGeneratorFactory : ICodeGeneratorFactory<(ITypeInformation Type, IMemberInformation KeyMember)>
{
   private static readonly InterfaceCodeGeneratorFactory _parsable = new(ParsableCodeGenerator.Default);
   private static readonly InterfaceCodeGeneratorFactory _parsableForValidatableEnum = new(ParsableCodeGenerator.ForValidatableEnum);
   private static readonly InterfaceCodeGeneratorFactory _comparable = new(ComparableCodeGenerator.Default);

   public static readonly InterfaceCodeGeneratorFactory Formattable = new(FormattableCodeGenerator.Instance);

   public static InterfaceCodeGeneratorFactory Comparable(string? comparerAccessor)
   {
      return String.IsNullOrWhiteSpace(comparerAccessor) ? _comparable : new InterfaceCodeGeneratorFactory(new ComparableCodeGenerator(comparerAccessor));
   }

   public static InterfaceCodeGeneratorFactory Parsable(bool forValidatableEnum)
   {
      return forValidatableEnum ? _parsableForValidatableEnum : _parsable;
   }

   public static InterfaceCodeGeneratorFactory Create(IInterfaceCodeGenerator codeGenerator)
   {
      return new InterfaceCodeGeneratorFactory(codeGenerator);
   }

   private readonly IInterfaceCodeGenerator _interfaceCodeGenerator;

   public string CodeGeneratorName => _interfaceCodeGenerator.CodeGeneratorName;

   private InterfaceCodeGeneratorFactory(IInterfaceCodeGenerator interfaceCodeGenerator)
   {
      _interfaceCodeGenerator = interfaceCodeGenerator;
   }

   public CodeGeneratorBase Create((ITypeInformation Type, IMemberInformation KeyMember) state, StringBuilder stringBuilder)
   {
      return new InterfaceCodeGenerator(_interfaceCodeGenerator, state.Type, state.KeyMember, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<(ITypeInformation Type, IMemberInformation KeyMember)> other)
   {
      return ReferenceEquals(this, other);
   }
}
