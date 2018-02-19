namespace Thinktecture.TestEnums
{
	public class IntegerEnum : EnumClass<IntegerEnum, int>
	{
		public static readonly IntegerEnum Item1 = new IntegerEnum(1);

		public IntegerEnum(int key)
			: base(key)
		{
		}

		protected override IntegerEnum CreateInvalid(int key)
		{
			return new IntegerEnum(key);
		}
	}
}
