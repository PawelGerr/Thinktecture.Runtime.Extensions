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
         ValidTestEnum calledActionOn = null;

         ValidTestEnum.Item1.SwitchPartially(item1: () =>
                                             {
                                                calledActionOn = ValidTestEnum.Item1;
                                             },
                                             item2: () =>
                                             {
                                                calledActionOn = ValidTestEnum.Item2;
                                             });

         calledActionOn.Should().Be(ValidTestEnum.Item1);
      }

      [Fact]
      public void Should_use_default_arg()
      {
         ValidTestEnum calledActionOn = null;

         ValidTestEnum.Item1.SwitchPartially(@default: item =>
                                             {
                                                calledActionOn = item;
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

         testItem.SwitchPartially(invalid: item =>
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
      public void Should_use_default_arg_having_validatable_class_enum(bool useInvalidItem)
      {
         TestEnum calledActionOn = null;
         var testItem = useInvalidItem ? TestEnum.Get("invalid item") : TestEnum.Item1;

         testItem.SwitchPartially(@default: item =>
                                  {
                                     calledActionOn = item;
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

         testItem.SwitchPartially(invalid: item =>
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
      public void Should_use_default_arg_having_struct()
      {
         TestSmartEnum_Struct_IntBased? calledActionOn = null;

         TestSmartEnum_Struct_IntBased.Value1.SwitchPartially(@default: item =>
                                                              {
                                                                 calledActionOn = item;
                                                              },
                                                              invalid: item =>
                                                              {
                                                                 calledActionOn = item;
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

         calledActionOn.Should().Be(TestSmartEnum_Struct_IntBased.Value1);
      }

      [Fact]
      public void Should_use_correct_arg_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         KeylessTestEnum.Item1.SwitchPartially(item1: () =>
                                               {
                                                  calledActionOn = KeylessTestEnum.Item1;
                                               },
                                               item2: () =>
                                               {
                                                  calledActionOn = KeylessTestEnum.Item2;
                                               });

         calledActionOn.Should().Be(KeylessTestEnum.Item1);
      }

      [Fact]
      public void Should_use_default_arg_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         KeylessTestEnum.Item1.SwitchPartially(@default: item =>
                                               {
                                                  calledActionOn = item;
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
      public void Should_pass_state()
      {
         ValidTestEnum calledActionOn = null;

         var obj = new object();

         ValidTestEnum.Item1.SwitchPartially(obj,
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

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         ValidTestEnum calledActionOn = null;

         var obj = new TestRefStruct(42);

         ValidTestEnum.Item1.SwitchPartially(obj,
                                             item1: o =>
                                                    {
                                                       o.Value.Should().Be(42);

                                                       calledActionOn = ValidTestEnum.Item1;
                                                    },
                                             item2: o =>
                                                    {
                                                       o.Value.Should().Be(42);

                                                       calledActionOn = ValidTestEnum.Item2;
                                                    });

         calledActionOn.Should().Be(ValidTestEnum.Item1);
      }
#endif

      [Fact]
      public void Should_pass_state_to_default()
      {
         ValidTestEnum calledActionOn = null;

         var obj = new object();

         ValidTestEnum.Item1.SwitchPartially(obj,
                                             @default: (o, item) =>
                                             {
                                                o.Should().Be(obj);

                                                calledActionOn = item;
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
      public void Should_pass_state_having_validatable_enum(bool useInvalidItem)
      {
         TestEnum calledActionOn = null;

         var obj = new object();
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(obj,
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
      public void Should_pass_state_to_default_having_validatable_enum(bool useInvalidItem)
      {
         TestEnum calledActionOn = null;

         var obj = new object();

         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(obj,
                                  @default: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     calledActionOn = item;
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
      public void Should_pass_state_having_struct(bool useInvalidItem)
      {
         TestSmartEnum_Struct_IntBased? calledActionOn = null;
         var obj = new object();

         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.SwitchPartially(obj,
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
      public void Should_pass_state_to_default_having_struct()
      {
         TestSmartEnum_Struct_IntBased? calledActionOn = null;
         var obj = new object();

         TestSmartEnum_Struct_IntBased.Value3.SwitchPartially(obj,
                                                              @default: (o, item) =>
                                                              {
                                                                 o.Should().Be(obj);
                                                                 calledActionOn = item;
                                                              },
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

         calledActionOn.Should().Be(TestSmartEnum_Struct_IntBased.Value3);
      }

      [Fact]
      public void Should_pass_state_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         var obj = new object();

         KeylessTestEnum.Item1.SwitchPartially(obj,
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

      [Fact]
      public void Should_pass_state_to_default_with_keyless_enum()
      {
         KeylessTestEnum calledActionOn = null;

         var obj = new object();

         KeylessTestEnum.Item1.SwitchPartially(obj,
                                               @default: (o, item) =>
                                               {
                                                  o.Should().Be(obj);

                                                  calledActionOn = item;
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
         ValidTestEnum.Item1.SwitchPartially(@default: item => item,
                                             item1: () => ValidTestEnum.Item1,
                                             item2: () => ValidTestEnum.Item2)
                      .Should().Be(ValidTestEnum.Item1);
      }

      [Fact]
      public void Should_call_default()
      {
         ValidTestEnum.Item1.SwitchPartially(@default: item => item,
                                             item2: () => ValidTestEnum.Item2)
                      .Should().Be(ValidTestEnum.Item1);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_call_correct_arg_having_validatable_enum(bool useInvalidItem)
      {
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(@default: item => item,
                                  invalid: item => item,
                                  item1: () => TestEnum.Item1,
                                  item2: () => TestEnum.Item2)
                 .Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_call_default_having_validatable_enum(bool useInvalidItem)
      {
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(@default: item => item,
                                  invalid: item => item,
                                  item2: () => TestEnum.Item2)
                 .Should().Be(testItem);
      }

      [Theory]
      [InlineData(true)]
      [InlineData(false)]
      public void Should_call_correct_arg_having_struct(bool useInvalidItem)
      {
         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.SwitchPartially(@default: item => item,
                                  invalid: item => item,
                                  value1: () => TestSmartEnum_Struct_IntBased.Value1,
                                  value2: () => TestSmartEnum_Struct_IntBased.Value2,
                                  value3: () => TestSmartEnum_Struct_IntBased.Value3,
                                  value4: () => TestSmartEnum_Struct_IntBased.Value4,
                                  value5: () => TestSmartEnum_Struct_IntBased.Value5)
                 .Should().Be(testItem);
      }

      [Fact]
      public void Should_call_default_having_struct()
      {
         TestSmartEnum_Struct_IntBased.Value3.SwitchPartially(@default: item => item,
                                                              invalid: item => item,
                                                              value1: () => TestSmartEnum_Struct_IntBased.Value1,
                                                              value2: () => TestSmartEnum_Struct_IntBased.Value2,
                                                              value4: () => TestSmartEnum_Struct_IntBased.Value4,
                                                              value5: () => TestSmartEnum_Struct_IntBased.Value5)
                                      .Should().Be(TestSmartEnum_Struct_IntBased.Value3);
      }

      [Fact]
      public void Should_call_correct_arg_with_keyless_enum()
      {
         KeylessTestEnum.Item1.SwitchPartially(@default: item => item,
                                               item1: () => KeylessTestEnum.Item1,
                                               item2: () => KeylessTestEnum.Item2)
                        .Should().Be(KeylessTestEnum.Item1);
      }

      [Fact]
      public void Should_call_default_with_keyless_enum()
      {
         KeylessTestEnum.Item1.SwitchPartially(@default: item => item,
                                               item2: () => KeylessTestEnum.Item2)
                        .Should().Be(KeylessTestEnum.Item1);
      }
   }

   public class WithFuncAndContext
   {
      [Fact]
      public void Should_pass_state()
      {
         var obj = new object();

         ValidTestEnum.Item1.SwitchPartially(obj,
                                             @default: (o, item) =>
                                             {
                                                o.Should().Be(obj);

                                                return item;
                                             },
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

#if NET9_0_OR_GREATER
      [Fact]
      public void Should_pass_state_having_ref_struct()
      {
         var obj = new TestRefStruct(42);

         ValidTestEnum.Item1.SwitchPartially(obj,
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

         ValidTestEnum.Item1.SwitchPartially(obj,
                                             @default: (o, item) =>
                                             {
                                                o.Should().Be(obj);

                                                return item;
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
      public void Should_pass_state_having_validatable_enum(bool useInvalidItem)
      {
         var obj = new object();
         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(obj,
                                  @default: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     return item;
                                  },
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
      public void Should_pass_state_to_default_having_validatable_enum(bool useInvalidItem)
      {
         var obj = new object();

         var testItem = useInvalidItem ? TestEnum.Get("invalid") : TestEnum.Item1;

         testItem.SwitchPartially(obj,
                                  @default: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     return item;
                                  },
                                  invalid: (o, item) =>
                                  {
                                     o.Should().Be(obj);

                                     return item;
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
      public void Should_pass_state_having_struct(bool useInvalidItem)
      {
         var obj = new object();

         var testItem = useInvalidItem ? TestSmartEnum_Struct_IntBased.Get(42) : TestSmartEnum_Struct_IntBased.Value3;

         testItem.SwitchPartially(obj,
                                  @default: (o, item) =>
                                  {
                                     o.Should().Be(obj);
                                     return item;
                                  },
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
      public void Should_pass_state_to_default_having_struct()
      {
         var obj = new object();

         TestSmartEnum_Struct_IntBased.Value3.SwitchPartially(obj,
                                                              @default: (o, item) =>
                                                              {
                                                                 o.Should().Be(obj);
                                                                 return item;
                                                              },
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
                                      .Should().Be(TestSmartEnum_Struct_IntBased.Value3);
      }

      [Fact]
      public void Should_pass_state_with_keyless_enum()
      {
         var obj = new object();

         KeylessTestEnum.Item1.SwitchPartially(obj,
                                               @default: (o, item) =>
                                               {
                                                  o.Should().Be(obj);
                                                  return item;
                                               },
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

      [Fact]
      public void Should_pass_state_to_default_with_keyless_enum()
      {
         var obj = new object();

         KeylessTestEnum.Item1.SwitchPartially(obj,
                                               @default: (o, item) =>
                                               {
                                                  o.Should().Be(obj);
                                                  return item;
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
