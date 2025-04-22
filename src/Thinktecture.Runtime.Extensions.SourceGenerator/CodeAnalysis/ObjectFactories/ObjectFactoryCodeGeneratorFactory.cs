using System.Text;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

public class ObjectFactoryCodeGeneratorFactory : ICodeGeneratorFactory<ObjectFactorySourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<ObjectFactorySourceGeneratorState> Instance = new ObjectFactoryCodeGeneratorFactory();

   public string CodeGeneratorName => "ObjectFactory-CodeGenerator";

   private ObjectFactoryCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(ObjectFactorySourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new ObjectFactoryCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<ObjectFactorySourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
