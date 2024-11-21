using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class DerivedTypesCodeGeneratorFactory : ICodeGeneratorFactory<SmartEnumDerivedTypes>
{
   public static readonly ICodeGeneratorFactory<SmartEnumDerivedTypes> Instance = new DerivedTypesCodeGeneratorFactory();

   public string CodeGeneratorName => "DerivedTypes-CodeGenerator";

   private DerivedTypesCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(SmartEnumDerivedTypes state, StringBuilder stringBuilder)
   {
      return new DerivedTypesCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<SmartEnumDerivedTypes> other)
   {
      return ReferenceEquals(this, other);
   }
}
