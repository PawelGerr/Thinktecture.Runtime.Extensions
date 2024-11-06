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
         ValidTestEnum calledActionOn = null;

         ValidTestEnum.Item1.Switch(item1: () =>
                                           {
                                              calledActionOn = ValidTestEnum.Item1;
                                           },
                                    item2: () =>
                                           {
                                              calledActionOn = ValidTestEnum.Item2;
                                           });

         calledActionOn.Should().Be(ValidTestEnum.Item1);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_use_correct_arg_having_validatable_class_enum(bool useInvalidItem)
      {
         TestEnum calledActionOn = null;
         var testItem = useInvalidItem ? TestEnum.Get("invalid item") : TestEnum.Item1;

         testItem.Switch(invalid: item =>
                                  {
                                     calledActionOn = item;
                                  },
                         item1: () =>
                                {
                                   calledActionOn = TestEnum.Item1;
                                },
                         item2: () =>
                                {
                                   calledActionOn = TestEnum.Item2;
                                });

         calledActionOn.Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_use_correct_arg_having_struct(bool useInvalidItem)
      {
         TestSmartEnum_Struct_IntBased? calledActionOn = null;

         var testItem = useInvalidItem ? (TestSmartEnum_Struct_IntBased)42 : TestSmartEnum_Struct_IntBased.Value3;

         testItem.Switch(invalid: item =>
                                  {
                                     calledActionOn = item;
                                  },
                         value1: () =>
                                 {
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value1;
                                 },
                         value2: () =>
                                 {
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value2;
                                 },
                         value3: () =>
                                 {
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value3;
                                 },
                         value4: () =>
                                 {
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value4;
                                 },
                         value5: () =>
                                 {
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value5;
                                 });

         calledActionOn.Should().Be(testItem);
      }

      [Fact]
      public void Should_use_correct_arg_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         KeylessTestEnum.Item1.Switch(item1: () =>
                                             {
                                                calledActionOn = KeylessTestEnum.Item1;
                                             },
                                      item2: () =>
                                             {
                                                calledActionOn = KeylessTestEnum.Item2;
                                             });

         calledActionOn.Should().Be(KeylessTestEnum.Item1);
      }
   }

   public class WithActionAndState
   {
      [Fact]
      public void Should_pass_context()
      {
         ValidTestEnum calledActionOn = null;

         var obj = new object();

         ValidTestEnum.Item1.Switch(obj,
                                    item1: o =>
                                           {
                                              o.Should().Be(obj);

                                              calledActionOn = ValidTestEnum.Item1;
                                           },
                                    item2: o =>
                                           {
                                              o.Should().Be(obj);

                                              calledActionOn = ValidTestEnum.Item2;
                                           });

         calledActionOn.Should().Be(ValidTestEnum.Item1);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_pass_context_having_validatable_enum(bool useInvalidItem)
      {
         TestEnum calledActionOn = null;

         var obj = new object();
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.Switch(obj,
                         invalid: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     calledActionOn = item;
                                  },
                         item1: o =>
                                {
                                   o.Should().Be(obj);

                                   calledActionOn = TestEnum.Item1;
                                },
                         item2: o =>
                                {
                                   o.Should().Be(obj);

                                   calledActionOn = TestEnum.Item2;
                                });

         calledActionOn.Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_pass_context_having_struct(bool useInvalidItem)
      {
         TestSmartEnum_Struct_IntBased? calledActionOn = null;
         var obj = new object();

         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.Switch(obj,
                         invalid: (o, item) =>
                                  {
                                     o.Should().Be(obj);
                                     calledActionOn = item;
                                  },
                         value1: o =>
                                 {
                                    o.Should().Be(obj);
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value1;
                                 },
                         value2: o =>
                                 {
                                    o.Should().Be(obj);
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value2;
                                 },
                         value3: o =>
                                 {
                                    o.Should().Be(obj);
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value3;
                                 },
                         value4: o =>
                                 {
                                    o.Should().Be(obj);
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value4;
                                 },
                         value5: o =>
                                 {
                                    o.Should().Be(obj);
                                    calledActionOn = TestSmartEnum_Struct_IntBased.Value5;
                                 });

         calledActionOn.Should().Be(testItem);
      }

      [Fact]
      public void Should_pass_context_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         var obj = new object();

         KeylessTestEnum.Item1.Switch(obj,
                                      item1: o =>
                                             {
                                                o.Should().Be(obj);

                                                calledActionOn = KeylessTestEnum.Item1;
                                             },
                                      item2: o =>
                                             {
                                                o.Should().Be(obj);

                                                calledActionOn = KeylessTestEnum.Item2;
                                             });

         calledActionOn.Should().Be(KeylessTestEnum.Item1);
      }
   }

   public class WithFunc
   {
      [Fact]
      public void Should_call_correct_arg()
      {
         ValidTestEnum.Item1.Switch(item1: () => ValidTestEnum.Item1,
                                    item2: () => ValidTestEnum.Item2)
                      .Should().Be(ValidTestEnum.Item1);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_call_correct_arg_having_validatable_enum(bool useInvalidItem)
      {
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.Switch(invalid: item => item,
                         item1: () => TestEnum.Item1,
                         item2: () => TestEnum.Item2)
                 .Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_call_correct_arg_having_struct(bool useInvalidItem)
      {
         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.Switch(invalid: item => item,
                         value1: () => TestSmartEnum_Struct_IntBased.Value1,
                         value2: () => TestSmartEnum_Struct_IntBased.Value2,
                         value3: () => TestSmartEnum_Struct_IntBased.Value3,
                         value4: () => TestSmartEnum_Struct_IntBased.Value4,
                         value5: () => TestSmartEnum_Struct_IntBased.Value5)
                 .Should().Be(testItem);
      }

      [Fact]
      public void Should_call_correct_arg_with_keyless_enum()
      {
         KeylessTestEnum.Item1.Switch(item1: () => KeylessTestEnum.Item1,
                                      item2: () => KeylessTestEnum.Item2)
                        .Should().Be(KeylessTestEnum.Item1);
      }
   }

   public class WithFuncAndContext
   {
      [Fact]
      public void Should_pass_context()
      {
         var obj = new object();

         ValidTestEnum.Item1.Switch(obj,
                                    item1: o =>
                                           {
                                              o.Should().Be(obj);

                                              return ValidTestEnum.Item1;
                                           },
                                    item2: o =>
                                           {
                                              o.Should().Be(obj);

                                              return ValidTestEnum.Item2;
                                           })
                      .Should().Be(ValidTestEnum.Item1);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_pass_context_having_validatable_enum(bool useInvalidItem)
      {
         var obj = new object();
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.Switch(obj,
                         invalid: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     return item;
                                  },
                         item1: o =>
                                {
                                   o.Should().Be(obj);

                                   return TestEnum.Item1;
                                },
                         item2: o =>
                                {
                                   o.Should().Be(obj);

                                   return TestEnum.Item2;
                                })
                 .Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_pass_context_having_struct(bool useInvalidItem)
      {
         var obj = new object();

         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.Switch(obj,
                         invalid: (o, item) =>
                                  {
                                     o.Should().Be(obj);
                                     return item;
                                  },
                         value1: o =>
                                 {
                                    o.Should().Be(obj);
                                    return TestSmartEnum_Struct_IntBased.Value1;
                                 },
                         value2: o =>
                                 {
                                    o.Should().Be(obj);
                                    return TestSmartEnum_Struct_IntBased.Value2;
                                 },
                         value3: o =>
                                 {
                                    o.Should().Be(obj);
                                    return TestSmartEnum_Struct_IntBased.Value3;
                                 },
                         value4: o =>
                                 {
                                    o.Should().Be(obj);
                                    return TestSmartEnum_Struct_IntBased.Value4;
                                 },
                         value5: o =>
                                 {
                                    o.Should().Be(obj);
                                    return TestSmartEnum_Struct_IntBased.Value5;
                                 })
                 .Should().Be(testItem);
      }

      [Fact]
      public void Should_pass_context_with_keyless_enum()
      {
         var obj = new object();

         KeylessTestEnum.Item1.Switch(obj,
                                      item1: o =>
                                             {
                                                o.Should().Be(obj);

                                                return KeylessTestEnum.Item1;
                                             },
                                      item2: o =>
                                             {
                                                o.Should().Be(obj);

                                                return KeylessTestEnum.Item2;
                                             })
                        .Should().Be(KeylessTestEnum.Item1);
      }
   }
}
