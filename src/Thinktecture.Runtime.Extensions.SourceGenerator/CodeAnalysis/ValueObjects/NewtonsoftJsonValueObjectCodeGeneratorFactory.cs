using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class NewtonsoftJsonValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly NewtonsoftJsonValueObjectCodeGeneratorFactory Instance = new();

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new NewtonsoftJsonValueObjectCodeGenerator(state, stringBuilder);
   }

   public override bool Equals(object? obj)
   {
      return obj is NewtonsoftJsonValueObjectCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is NewtonsoftJsonValueObjectCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
