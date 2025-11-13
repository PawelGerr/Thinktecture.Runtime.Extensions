using System.Runtime.CompilerServices;
using System.Text;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class ObjectFactoryCodeGeneratorFactory
   : ICodeGeneratorFactory<ObjectFactorySourceGeneratorState>,
     IEquatable<ObjectFactoryCodeGeneratorFactory>
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

   public bool Equals(ICodeGeneratorFactory<ObjectFactorySourceGeneratorState>? other) => ReferenceEquals(this, other);
   public bool Equals(ObjectFactoryCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
