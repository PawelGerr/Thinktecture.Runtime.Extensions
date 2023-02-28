using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Switch
{
   [Fact]
   public void Should_call_correct_action()
   {
      TestEnum calledActionOn = null;

      TestEnum.Item1.Switch(TestEnum.Item1, () => calledActionOn = TestEnum.Item1,
                            TestEnum.Item2, () => calledActionOn = TestEnum.Item2);

      calledActionOn.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_pass_context_to_action()
   {
      TestEnum calledActionOn = null;

      var obj = new object();

      TestEnum.Item1.Switch(obj,
                            TestEnum.Item1, o =>
                                            {
                                               o.Should().Be(obj);

                                               calledActionOn = TestEnum.Item1;
                                            },
                            TestEnum.Item2, o =>
                                            {
                                               o.Should().Be(obj);

                                               calledActionOn = TestEnum.Item2;
                                            });

      calledActionOn.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_call_correct_func()
   {
      TestEnum.Item1.Switch(TestEnum.Item1, () => TestEnum.Item1,
                            TestEnum.Item2, () => TestEnum.Item2)
              .Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_pass_context_to_func()
   {
      var obj = new object();

      TestEnum.Item1.Switch(obj,
                            TestEnum.Item1, o =>
                                            {
                                               o.Should().Be(obj);

                                               return TestEnum.Item1;
                                            },
                            TestEnum.Item2, o =>
                                            {
                                               o.Should().Be(obj);

                                               return TestEnum.Item2;
                                            })
              .Should().Be(TestEnum.Item1);
   }
}
