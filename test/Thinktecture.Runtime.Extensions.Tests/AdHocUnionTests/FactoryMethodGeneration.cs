using Thinktecture.Runtime.Tests.TestAdHocUnions;

namespace Thinktecture.Runtime.Tests.AdHocUnionTests;

// ReSharper disable once InconsistentNaming
public class FactoryMethodGeneration
{
   public class Always
   {
      [Fact]
      public void Should_create_string_member_via_factory_method()
      {
         var union = TestUnion_class_string_int_with_FactoryMethodGeneration_Always.CreateString("hello");

         union.IsString.Should().BeTrue();
         union.IsInt32.Should().BeFalse();
         union.AsString.Should().Be("hello");
         union.Value.Should().Be("hello");
      }

      [Fact]
      public void Should_create_int_member_via_factory_method()
      {
         var union = TestUnion_class_string_int_with_FactoryMethodGeneration_Always.CreateInt32(42);

         union.IsInt32.Should().BeTrue();
         union.IsString.Should().BeFalse();
         union.AsInt32.Should().Be(42);
         union.Value.Should().Be(42);
      }

      [Fact]
      public void Should_still_have_public_constructors()
      {
         var stringUnion = new TestUnion_class_string_int_with_FactoryMethodGeneration_Always("hello");
         stringUnion.IsString.Should().BeTrue();
         stringUnion.AsString.Should().Be("hello");

         var intUnion = new TestUnion_class_string_int_with_FactoryMethodGeneration_Always(42);
         intUnion.IsInt32.Should().BeTrue();
         intUnion.AsInt32.Should().Be(42);
      }

      [Fact]
      public void Should_still_have_implicit_conversion_operators()
      {
         TestUnion_class_string_int_with_FactoryMethodGeneration_Always stringUnion = "text";
         stringUnion.IsString.Should().BeTrue();

         TestUnion_class_string_int_with_FactoryMethodGeneration_Always intUnion = 42;
         intUnion.IsInt32.Should().BeTrue();
      }

      [Fact]
      public void Should_create_string_member_with_null_value()
      {
         var union = TestUnion_class_string_int_with_FactoryMethodGeneration_Always.CreateString(null!);

         union.IsString.Should().BeTrue();
         union.AsString.Should().BeNull();
      }
   }

   public class None
   {
      [Fact]
      public void Should_have_public_constructors()
      {
         var stringUnion = new TestUnion_class_string_int_with_FactoryMethodGeneration_None("hello");
         stringUnion.IsString.Should().BeTrue();
         stringUnion.AsString.Should().Be("hello");
         stringUnion.Value.Should().Be("hello");

         var intUnion = new TestUnion_class_string_int_with_FactoryMethodGeneration_None(42);
         intUnion.IsInt32.Should().BeTrue();
         intUnion.AsInt32.Should().Be(42);
         intUnion.Value.Should().Be(42);
      }

      [Fact]
      public void Should_still_have_implicit_conversion_operators()
      {
         TestUnion_class_string_int_with_FactoryMethodGeneration_None stringUnion = "text";
         stringUnion.IsString.Should().BeTrue();
         stringUnion.AsString.Should().Be("text");

         TestUnion_class_string_int_with_FactoryMethodGeneration_None intUnion = 42;
         intUnion.IsInt32.Should().BeTrue();
         intUnion.AsInt32.Should().Be(42);
      }
   }
}
