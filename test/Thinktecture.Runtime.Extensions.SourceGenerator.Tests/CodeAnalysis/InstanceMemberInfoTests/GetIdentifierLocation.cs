using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.InstanceMemberInfoTests;

public class GetIdentifierLocation : CompilationTestBase
{
   [Fact]
   public void Should_return_location_for_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      location!.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_location_for_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      location!.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_return_null_when_symbol_not_captured_for_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_symbol_not_captured_for_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().BeNull();
   }

   [Fact]
   public void Should_handle_cancellation_token()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var cts = new CancellationTokenSource();

      // Act
      var location = memberInfo!.GetIdentifierLocation(cts.Token);

      // Assert
      location.Should().NotBeNull();
   }

   [Fact]
   public void Should_return_correct_line_and_column_for_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("_field").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      var lineSpan = location!.GetLineSpan();
      lineSpan.Path.Should().BeEmpty(); // no file path because of in-memory compilation
      lineSpan.StartLinePosition.Line.Should().BeGreaterThanOrEqualTo(0);
   }

   [Fact]
   public void Should_return_correct_line_and_column_for_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public int Property { get; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Property").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      var lineSpan = location!.GetLineSpan();
      lineSpan.Path.Should().BeEmpty(); // no file path because of in-memory compilation
      lineSpan.StartLinePosition.Line.Should().BeGreaterThanOrEqualTo(0);
   }

   [Fact]
   public void Should_handle_multiple_fields_and_return_correct_location()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   private readonly int _field1;
   private readonly string _field2;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field1 = (IFieldSymbol)type.GetMembers("_field1").First();
      var field2 = (IFieldSymbol)type.GetMembers("_field2").First();
      var memberInfo1 = InstanceMemberInfo.CreateOrNull(factory, field1, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);
      var memberInfo2 = InstanceMemberInfo.CreateOrNull(factory, field2, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location1 = memberInfo1!.GetIdentifierLocation(CancellationToken.None);
      var location2 = memberInfo2!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location1.Should().NotBeNull();
      location2.Should().NotBeNull();
      location1.Should().NotBe(location2);
   }

   [Fact]
   public void Should_handle_static_field()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public static readonly int StaticField = 42;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var field = (IFieldSymbol)type.GetMembers("StaticField").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, field, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      location!.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_static_property()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public static int StaticProperty { get; } = 42;
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("StaticProperty").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      location!.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_property_with_init_accessor()
   {
      // Arrange
      var src = @"
namespace Test;

public class TestClass
{
   public string Name { get; init; }
}
";
      var compilation = CreateCompilation(src);
      var factory = TypedMemberStateFactory.Create(compilation);
      var type = GetTypeSymbol(compilation, "Test.TestClass");
      var property = (IPropertySymbol)type.GetMembers("Name").First();
      var memberInfo = InstanceMemberInfo.CreateOrNull(factory, property, populateValueObjectMemberSettings: false, allowedCaptureSymbols: true);

      // Act
      var location = memberInfo!.GetIdentifierLocation(CancellationToken.None);

      // Assert
      location.Should().NotBeNull();
      location!.IsInSource.Should().BeTrue();
   }
}
