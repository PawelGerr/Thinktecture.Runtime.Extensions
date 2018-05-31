using Xunit;

namespace Thinktecture.EmptyActionTests
{
	public class EmptyAction
	{
		// no idea how to test for "does nothing"
		[Fact]
		public void Should_do_nothing()
		{
			Empty.Action();
			Empty.Action(0);
			Empty.Action(0, 0);
			Empty.Action(0, 0, 0);
			Empty.Action(0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Empty.Action(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		}
	}
}
