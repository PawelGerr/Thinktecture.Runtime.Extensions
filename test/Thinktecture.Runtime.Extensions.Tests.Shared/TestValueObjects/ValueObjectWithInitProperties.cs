namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public sealed partial class ValueObjectWithInitProperties
{
   [ValueObjectMemberIgnore]
   private readonly int _initExpression;

   public int InitExpression
   {
      get => _initExpression;
      private init => _initExpression = value;
   }

   [ValueObjectMemberIgnore]
   private readonly int _initBody;

   public int InitBody
   {
      get { return _initBody; }
      private init { _initBody = value; }
   }

   public int PublicPropertyDefaultInit { get; private init; }
   private int PrivatePropertyDefaultInit { get; init; }
}
