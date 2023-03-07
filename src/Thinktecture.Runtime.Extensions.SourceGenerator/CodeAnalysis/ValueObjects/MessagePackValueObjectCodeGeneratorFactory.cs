using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MessagePackValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly MessagePackValueObjectCodeGeneratorFactory Instance = new();

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new MessagePackValueObjectCodeGenerator(state, stringBuilder);
   }

   public override bool Equals(object? obj)
   {
      return obj is MessagePackValueObjectCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is MessagePackValueObjectCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
