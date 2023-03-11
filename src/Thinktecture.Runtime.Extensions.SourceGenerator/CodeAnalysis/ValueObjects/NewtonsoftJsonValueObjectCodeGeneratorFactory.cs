using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class NewtonsoftJsonValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<ValueObjectSourceGeneratorState> Instance = new NewtonsoftJsonValueObjectCodeGeneratorFactory();

   private NewtonsoftJsonValueObjectCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new NewtonsoftJsonValueObjectCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is NewtonsoftJsonValueObjectCodeGeneratorFactory;
   }
}
