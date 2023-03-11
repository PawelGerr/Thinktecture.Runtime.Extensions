using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MessagePackValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<ValueObjectSourceGeneratorState> Instance = new MessagePackValueObjectCodeGeneratorFactory();

   private MessagePackValueObjectCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new MessagePackValueObjectCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is MessagePackValueObjectCodeGeneratorFactory;
   }
}
