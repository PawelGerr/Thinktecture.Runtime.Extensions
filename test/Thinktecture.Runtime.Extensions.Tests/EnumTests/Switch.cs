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
   public void Should_call_correct_action_with_keyless_enum()
   {
      KeylessTestEnum calledActionOn = null;

      KeylessTestEnum.Item1.Switch(KeylessTestEnum.Item1, () => calledActionOn = KeylessTestEnum.Item1,
                                   KeylessTestEnum.Item2, () => calledActionOn = KeylessTestEnum.Item2);

      calledActionOn.Should().Be(KeylessTestEnum.Item1);
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
   public void Should_pass_context_to_action_with_keyless_enum()
   {
      KeylessTestEnum calledActionOn = null;

      var obj = new object();

      KeylessTestEnum.Item1.Switch(obj,
                                   KeylessTestEnum.Item1, o =>
                                                          {
                                                             o.Should().Be(obj);

                                                             calledActionOn = KeylessTestEnum.Item1;
                                                          },
                                   KeylessTestEnum.Item2, o =>
                                                          {
                                                             o.Should().Be(obj);

                                                             calledActionOn = KeylessTestEnum.Item2;
                                                          });

      calledActionOn.Should().Be(KeylessTestEnum.Item1);
   }

   [Fact]
   public void Should_call_correct_func()
   {
      TestEnum.Item1.Switch(TestEnum.Item1, () => TestEnum.Item1,
                            TestEnum.Item2, () => TestEnum.Item2)
              .Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_call_correct_func_with_keyless_enum()
   {
      KeylessTestEnum.Item1.Switch(KeylessTestEnum.Item1, () => KeylessTestEnum.Item1,
                                   KeylessTestEnum.Item2, () => KeylessTestEnum.Item2)
                     .Should().Be(KeylessTestEnum.Item1);
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

   [Fact]
   public void Should_pass_context_to_func_with_keyless_enum()
   {
      var obj = new object();

      KeylessTestEnum.Item1.Switch(obj,
                                   KeylessTestEnum.Item1, o =>
                                                          {
                                                             o.Should().Be(obj);

                                                             return KeylessTestEnum.Item1;
                                                          },
                                   KeylessTestEnum.Item2, o =>
                                                          {
                                                             o.Should().Be(obj);

                                                             return KeylessTestEnum.Item2;
                                                          })
                     .Should().Be(KeylessTestEnum.Item1);
   }
}
