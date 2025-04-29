using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class SwitchPartially
{
   public class WithAction
   {
      [Fact]
      public void Should_use_correct_arg()
      {
         SmartEnum_StringBased_SwitchMapPartially calledActionOn = null;

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(item1: () =>
                                                                        {
                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item1;
                                                                        },
                                                                        item2: () =>
                                                                        {
                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        });

         calledActionOn.Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_use_default_arg()
      {
         SmartEnum_StringBased_SwitchMapPartially calledActionOn = null;

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(@default: item =>
                                                                        {
                                                                           calledActionOn = item;
                                                                        },
                                                                        item2: () =>
                                                                        {
                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        });

         calledActionOn.Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_use_correct_arg_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         SmartEnum_Keyless.Item1.SwitchPartially(item1: () =>
                                                 {
                                                    calledActionOn = SmartEnum_Keyless.Item1;
                                                 },
                                                 item2: () =>
                                                 {
                                                    calledActionOn = SmartEnum_Keyless.Item2;
                                                 });

         calledActionOn.Should().Be(SmartEnum_Keyless.Item1);
      }

      [Fact]
      public void Should_use_default_arg_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         SmartEnum_Keyless.Item1.SwitchPartially(@default: item =>
                                                 {
                                                    calledActionOn = item;
                                                 },
                                                 item2: () =>
                                                 {
                                                    calledActionOn = SmartEnum_Keyless.Item2;
                                                 });

         calledActionOn.Should().Be(SmartEnum_Keyless.Item1);
      }
   }

   public class WithActionAndState
   {
      [Fact]
      public void Should_pass_state()
      {
         SmartEnum_StringBased_SwitchMapPartially calledActionOn = null;

         var obj = new object();

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        item1: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item1;
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        });

         calledActionOn.Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         SmartEnum_StringBased_SwitchMapPartially calledActionOn = null;

         var obj = new TestRefStruct(42);

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        item1: o =>
                                                                        {
                                                                           o.Value.Should().Be(42);

                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item1;
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Value.Should().Be(42);

                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        });

         calledActionOn.Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }
#endif

      [Fact]
      public void Should_pass_state_to_default()
      {
         SmartEnum_StringBased_SwitchMapPartially calledActionOn = null;

         var obj = new object();

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        @default: (o, item) =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           calledActionOn = item;
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           calledActionOn = SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        });

         calledActionOn.Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_pass_state_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         var obj = new object();

         SmartEnum_Keyless.Item1.SwitchPartially(obj,
                                                 item1: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    calledActionOn = SmartEnum_Keyless.Item1;
                                                 },
                                                 item2: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    calledActionOn = SmartEnum_Keyless.Item2;
                                                 });

         calledActionOn.Should().Be(SmartEnum_Keyless.Item1);
      }

      [Fact]
      public void Should_pass_state_to_default_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         var obj = new object();

         SmartEnum_Keyless.Item1.SwitchPartially(obj,
                                                 @default: (o, item) =>
                                                 {
                                                    o.Should().Be(obj);

                                                    calledActionOn = item;
                                                 },
                                                 item2: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    calledActionOn = SmartEnum_Keyless.Item2;
                                                 });

         calledActionOn.Should().Be(SmartEnum_Keyless.Item1);
      }
   }

   public class WithFunc
   {
      [Fact]
      public void Should_call_correct_arg()
      {
         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(@default: item => item,
                                                                        item1: () => SmartEnum_StringBased_SwitchMapPartially.Item1,
                                                                        item2: () => SmartEnum_StringBased_SwitchMapPartially.Item2)
                                                 .Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_call_default()
      {
         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(@default: item => item,
                                                                        item2: () => SmartEnum_StringBased_SwitchMapPartially.Item2)
                                                 .Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_call_correct_arg_with_keyless_enum()
      {
         SmartEnum_Keyless.Item1.SwitchPartially(@default: item => item,
                                                 item1: () => SmartEnum_Keyless.Item1,
                                                 item2: () => SmartEnum_Keyless.Item2)
                          .Should().Be(SmartEnum_Keyless.Item1);
      }

      [Fact]
      public void Should_call_default_with_keyless_enum()
      {
         SmartEnum_Keyless.Item1.SwitchPartially(@default: item => item,
                                                 item2: () => SmartEnum_Keyless.Item2)
                          .Should().Be(SmartEnum_Keyless.Item1);
      }
   }

   public class WithFuncAndContext
   {
      [Fact]
      public void Should_pass_state()
      {
         var obj = new object();

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        @default: (o, item) =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           return item;
                                                                        },
                                                                        item1: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           return SmartEnum_StringBased_SwitchMapPartially.Item1;
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           return SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        })
                                                 .Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         var obj = new TestRefStruct(42);

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        @default: (o, item) =>
                                                                        {
                                                                           o.Value.Should().Be(42);

                                                                           return new TestRefStruct(-1);
                                                                        },
                                                                        item1: o =>
                                                                        {
                                                                           o.Value.Should().Be(42);

                                                                           return new TestRefStruct(1);
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Value.Should().Be(42);

                                                                           return new TestRefStruct(2);
                                                                        })
                                                 .Value.Should().Be(1);
      }
#endif

      [Fact]
      public void Should_pass_state_to_default()
      {
         var obj = new object();

         SmartEnum_StringBased_SwitchMapPartially.Item1.SwitchPartially(obj,
                                                                        @default: (o, item) =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           return item;
                                                                        },
                                                                        item2: o =>
                                                                        {
                                                                           o.Should().Be(obj);

                                                                           return SmartEnum_StringBased_SwitchMapPartially.Item2;
                                                                        })
                                                 .Should().Be(SmartEnum_StringBased_SwitchMapPartially.Item1);
      }

      [Fact]
      public void Should_pass_state_with_keyless_enum()
      {
         var obj = new object();

         SmartEnum_Keyless.Item1.SwitchPartially(obj,
                                                 @default: (o, item) =>
                                                 {
                                                    o.Should().Be(obj);
                                                    return item;
                                                 },
                                                 item1: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    return SmartEnum_Keyless.Item1;
                                                 },
                                                 item2: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    return SmartEnum_Keyless.Item2;
                                                 })
                          .Should().Be(SmartEnum_Keyless.Item1);
      }

      [Fact]
      public void Should_pass_state_to_default_with_keyless_enum()
      {
         var obj = new object();

         SmartEnum_Keyless.Item1.SwitchPartially(obj,
                                                 @default: (o, item) =>
                                                 {
                                                    o.Should().Be(obj);
                                                    return item;
                                                 },
                                                 item2: o =>
                                                 {
                                                    o.Should().Be(obj);

                                                    return SmartEnum_Keyless.Item2;
                                                 })
                          .Should().Be(SmartEnum_Keyless.Item1);
      }
   }
}
