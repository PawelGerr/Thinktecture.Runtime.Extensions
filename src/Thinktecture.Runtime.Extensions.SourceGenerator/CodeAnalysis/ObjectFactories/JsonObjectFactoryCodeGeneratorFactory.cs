using System.Runtime.CompilerServices;

namespace Thinktecture.CodeAnalysis.ObjectFactories;

public sealed class JsonObjectFactoryCodeGeneratorFactory
   : JsonKeyedSerializerCodeGeneratorFactoryBase,
     IEquatable<JsonObjectFactoryCodeGeneratorFactory>
{
   public static readonly IKeyedSerializerCodeGeneratorFactory Instance = new JsonObjectFactoryCodeGeneratorFactory();

   public override string CodeGeneratorName => "SystemTextJson-ObjectFactory-CodeGenerator";

   private JsonObjectFactoryCodeGeneratorFactory()
      : base(true)
   {
   }

   public bool Equals(JsonObjectFactoryCodeGeneratorFactory? other) => ReferenceEquals(this, other);
   public override bool Equals(object? obj) => ReferenceEquals(this, obj);

   public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
}
