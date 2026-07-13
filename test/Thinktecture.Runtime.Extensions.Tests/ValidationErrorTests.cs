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
}
