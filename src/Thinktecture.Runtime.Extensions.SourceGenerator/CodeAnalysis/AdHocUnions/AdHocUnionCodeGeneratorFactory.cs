using System.Text;

namespace Thinktecture.CodeAnalysis.AdHocUnions;

public class AdHocUnionCodeGeneratorFactory : ICodeGeneratorFactory<AdHocUnionSourceGenState>
{
   public static readonly ICodeGeneratorFactory<AdHocUnionSourceGenState> Instance = new AdHocUnionCodeGeneratorFactory();

   public string CodeGeneratorName => "AdHocUnion-CodeGenerator";

   private AdHocUnionCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(AdHocUnionSourceGenState state, StringBuilder stringBuilder)
   {
      return new AdHocUnionCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<AdHocUnionSourceGenState> other)
   {
      return ReferenceEquals(this, other);
   }
}
