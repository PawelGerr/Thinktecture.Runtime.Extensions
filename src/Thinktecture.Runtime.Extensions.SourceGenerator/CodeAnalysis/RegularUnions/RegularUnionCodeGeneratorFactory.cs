using System.Text;

namespace Thinktecture.CodeAnalysis.RegularUnions;

public class RegularUnionCodeGeneratorFactory : ICodeGeneratorFactory<RegularUnionSourceGenState>
{
   public static readonly ICodeGeneratorFactory<RegularUnionSourceGenState> Instance = new RegularUnionCodeGeneratorFactory();

   public string CodeGeneratorName => "RegularUnion-CodeGenerator";

   private RegularUnionCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(RegularUnionSourceGenState state, StringBuilder stringBuilder)
   {
      return new RegularUnionCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<RegularUnionSourceGenState> other)
   {
      return ReferenceEquals(this, other);
   }
}
