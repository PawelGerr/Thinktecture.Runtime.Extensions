namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class ValueObjectWithInitProperties
{
   [IgnoreMemberAttribute]
   private readonly int _initExpression;

   public int InitExpression
   {
      get => _initExpression;
      private init => _initExpression = value;
   }

   [IgnoreMemberAttribute]
   private readonly int _initBody;

   public int InitBody
   {
      get { return _initBody; }
      private init { _initBody = value; }
   }

   public int PublicPropertyDefaultInit { get; private init; }
   private int PrivatePropertyDefaultInit { get; init; }
}
