using System.Threading.Tasks;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class Switch
{
   public class WithAction
   {
      [Fact]
      public void Should_use_correct_arg()
      {
         SmartEnum_StringBased calledActionOn = null;

         SmartEnum_StringBased.Item1.Switch(item1: () =>
                                            {
                                               calledActionOn = SmartEnum_StringBased.Item1;
                                            },
                                            item2: () =>
                                            {
                                               calledActionOn = SmartEnum_StringBased.Item2;
                                            });

         calledActionOn.Should().Be(SmartEnum_StringBased.Item1);
      }

      [Fact]
      public void Should_use_correct_arg_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         SmartEnum_Keyless.Item1.Switch(item1: () =>
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
      public void Should_not_report_issue_when_using_disposables()
      {
         using var disposable = Empty.Disposable();
         SmartEnum_Keyless calledActionOn = null;

         SmartEnum_Keyless.Item1.Switch(item1: () =>
                                        {
                                           _ = disposable;
                                           calledActionOn = SmartEnum_Keyless.Item1;
                                        },
                                        item2: () =>
                                        {
                                           _ = disposable;
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
         SmartEnum_StringBased calledActionOn = null;

         var obj = new object();

         SmartEnum_StringBased.Item1.Switch(obj,
                                            item1: o =>
                                            {
                                               o.Should().Be(obj);

                                               calledActionOn = SmartEnum_StringBased.Item1;
                                            },
                                            item2: o =>
                                            {
                                               o.Should().Be(obj);

                                               calledActionOn = SmartEnum_StringBased.Item2;
                                            });

         calledActionOn.Should().Be(SmartEnum_StringBased.Item1);
      }

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         SmartEnum_StringBased calledActionOn = null;

         var obj = new TestRefStruct(42);

         SmartEnum_StringBased.Item1.Switch(obj,
                                            item1: o =>
                                            {
                                               o.Value.Should().Be(42);

                                               calledActionOn = SmartEnum_StringBased.Item1;
                                            },
                                            item2: o =>
                                            {
                                               o.Value.Should().Be(42);

                                               calledActionOn = SmartEnum_StringBased.Item2;
                                            });

         calledActionOn.Should().Be(SmartEnum_StringBased.Item1);
      }
#endif

      [Fact]
      public void Should_pass_state_with_keyless_enum()
      {
         SmartEnum_Keyless calledActionOn = null;

         var obj = new object();

         SmartEnum_Keyless.Item1.Switch(obj,
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
   }

   public class WithFunc
   {
      [Fact]
      public void Should_call_correct_arg()
      {
         SmartEnum_StringBased.Item1.Switch(item1: () => SmartEnum_StringBased.Item1,
                                            item2: () => SmartEnum_StringBased.Item2)
                              .Should().Be(SmartEnum_StringBased.Item1);
      }

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_call_correct_arg_and_return_ref_struct()
      {
         SmartEnum_StringBased.Item1.Switch(item1: () => new TestRefStruct(1),
                                            item2: () => new TestRefStruct(2))
                              .Value.Should().Be(1);
      }
#endif

      [Fact]
      public void Should_call_correct_arg_with_keyless_enum()
      {
         SmartEnum_Keyless.Item1.Switch(item1: () => SmartEnum_Keyless.Item1,
                                        item2: () => SmartEnum_Keyless.Item2)
                          .Should().Be(SmartEnum_Keyless.Item1);
      }

      [Fact]
      public async Task Should_not_report_issue_when_using_disposables()
      {
         using var disposable = Empty.Disposable();

         (await SmartEnum_Keyless.Item1.Switch(item1: async () =>
                                               {
                                                  _ = disposable;

                                                  await Task.Delay(1);

                                                  return SmartEnum_Keyless.Item1;
                                               },
                                               item2: async () =>
                                               {
                                                  _ = disposable;

                                                  await Task.Delay(1);

                                                  return SmartEnum_Keyless.Item2;
                                               }))
            .Should().Be(SmartEnum_Keyless.Item1);
      }
   }

   public class WithFuncAndContext
   {
      [Fact]
      public void Should_pass_state()
      {
         var obj = new object();

         SmartEnum_StringBased.Item1.Switch(obj,
                                            item1: o =>
                                            {
                                               o.Should().Be(obj);

                                               return SmartEnum_StringBased.Item1;
                                            },
                                            item2: o =>
                                            {
                                               o.Should().Be(obj);

                                               return SmartEnum_StringBased.Item2;
                                            })
                              .Should().Be(SmartEnum_StringBased.Item1);
      }

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         var obj = new TestRefStruct(42);

         SmartEnum_StringBased.Item1.Switch(obj,
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
      public void Should_pass_state_with_keyless_enum()
      {
         var obj = new object();

         SmartEnum_Keyless.Item1.Switch(obj,
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
   }
}
