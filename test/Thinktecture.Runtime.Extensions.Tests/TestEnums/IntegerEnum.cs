namespace Thinktecture.TestEnums
{
	public class IntegerEnum : Enum<IntegerEnum, int>
	{
		public static readonly IntegerEnum Item1 = new(1);

		public IntegerEnum(int key)
			: base(key)
		{
		}

		protected override IntegerEnum CreateInvalid(int key)
		{
			return new(key);
		}
	}
}
