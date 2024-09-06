using System.Text;

namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public class UnionCodeGeneratorFactory : ICodeGeneratorFactory<UnionSourceGenState>
{
   public static readonly ICodeGeneratorFactory<UnionSourceGenState> Instance = new UnionCodeGeneratorFactory();

   public string CodeGeneratorName => "Union-CodeGenerator";

   private UnionCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(UnionSourceGenState state, StringBuilder stringBuilder)
   {
      return new UnionCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<UnionSourceGenState> other)
   {
      return ReferenceEquals(this, other);
   }
}
