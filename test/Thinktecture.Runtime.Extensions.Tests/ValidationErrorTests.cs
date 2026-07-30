namespace Thinktecture.Runtime.Tests;

// ReSharper disable SuspiciousTypeConversion.Global
public class ValidationErrorTests
{
   [Fact]
   public void Should_return_false_when_comparing_with_foreign_type_via_object_Equals()
   {
      object error = new ValidationError("some message");

      // Regression: Equals(object?) must not throw for incompatible types (object.Equals contract).
      error.Equals("some message").Should().BeFalse();
      error.Equals(42).Should().BeFalse();
      error.Equals(new object()).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_comparing_with_null()
   {
      object error = new ValidationError("some message");

      error.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_comparing_with_equal_ValidationError()
   {
      object error = new ValidationError("some message");
      object other = new ValidationError("some message");

      error.Equals(other).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_comparing_equal_instances_via_equality_operator()
   {
      var left = new ValidationError("some message");
      var right = new ValidationError("some message");

      (left == right).Should().BeTrue();
      (left != right).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_comparing_different_instances_via_equality_operator()
   {
      var left = new ValidationError("some message");
      var right = new ValidationError("other message");

      (left == right).Should().BeFalse();
      (left != right).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_comparing_nulls_via_equality_operator()
   {
      ((ValidationError?)null == null).Should().BeTrue();
      ((ValidationError?)null != null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_comparing_instance_with_null_via_equality_operator()
   {
      var error = new ValidationError("some message");

      (error == null).Should().BeFalse();
      (null == error).Should().BeFalse();
      (error != null).Should().BeTrue();
      (null != error).Should().BeTrue();
   }
}
