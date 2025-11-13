using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.EventSymbolExtensionsTests;

public class GetEventLocation : CompilationTestBase
{
   [Fact]
   public void Should_return_identifier_location_for_EventDeclarationSyntax()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler MyEvent
   {
      add { }
      remove { }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("MyEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var lineSpan = location.GetLineSpan();
      lineSpan.Path.Should().NotContain(".g.cs");
      lineSpan.Path.Should().NotContain(".generated.cs");
   }

   [Fact]
   public void Should_return_identifier_location_for_event_field_with_single_declarator()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler SingleEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("SingleEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var lineSpan = location.GetLineSpan();
      lineSpan.Path.Should().NotContain(".g.cs");
   }

   [Fact]
   public void Should_return_identifier_location_for_first_event_in_multiple_declarators()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler Event1, Event2, Event3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var event1Symbol = type.GetMembers("Event1").OfType<IEventSymbol>().Single();

      var location = event1Symbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText().ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Event1");
   }

   [Fact]
   public void Should_return_identifier_location_for_second_event_in_multiple_declarators()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler Event1, Event2, Event3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var event2Symbol = type.GetMembers("Event2").OfType<IEventSymbol>().Single();

      var location = event2Symbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText().ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Event2");
   }

   [Fact]
   public void Should_return_identifier_location_for_third_event_in_multiple_declarators()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler Event1, Event2, Event3;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var event3Symbol = type.GetMembers("Event3").OfType<IEventSymbol>().Single();

      var location = event3Symbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText().ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("Event3");
   }

   [Fact]
   public void Should_skip_generated_trees_and_return_non_generated_location()
   {
      // Create a compilation with two syntax trees - one generated, one not
      var userSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler UserEvent;
}
";
      var generatedSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler GeneratedEvent;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs");
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userEvent = type!.GetMembers("UserEvent").OfType<IEventSymbol>().Single();

      var location = userEvent.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".g.cs");
   }

   [Fact]
   public void Should_return_Location_None_when_all_trees_are_generated()
   {
      var generatedSource = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler GeneratedEvent;
}
";
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var generatedEvent = type!.GetMembers("GeneratedEvent").OfType<IEventSymbol>().Single();

      var location = generatedEvent.GetEventLocation(CancellationToken.None);

      // When all trees are generated, we should get Location.None
      location.Should().Be(Location.None);
   }

   [Fact]
   public void Should_handle_event_with_delegate_type()
   {
      var src = @"
namespace Test;

public delegate void MyDelegate(int value);

public class MyClass
{
   public event MyDelegate CustomEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("CustomEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_static_event()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public static event EventHandler StaticEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("StaticEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_protected_event()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   protected event EventHandler ProtectedEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("ProtectedEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_internal_event()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   internal event EventHandler InternalEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("InternalEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_private_event()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   private event EventHandler PrivateEvent;

   public void UseEvent()
   {
      PrivateEvent?.Invoke(this, EventArgs.Empty);
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("PrivateEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_abstract_event()
   {
      var src = @"
using System;

namespace Test;

public abstract class MyClass
{
   public abstract event EventHandler AbstractEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("AbstractEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_virtual_event()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public virtual event EventHandler VirtualEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("VirtualEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_override_event()
   {
      var src = @"
using System;

namespace Test;

public class BaseClass
{
   public virtual event EventHandler VirtualEvent;
}

public class DerivedClass : BaseClass
{
   public override event EventHandler VirtualEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.DerivedClass");
      var eventSymbol = type.GetMembers("VirtualEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_event_in_interface()
   {
      var src = @"
using System;

namespace Test;

public interface IMyInterface
{
   event EventHandler InterfaceEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.IMyInterface");
      var eventSymbol = type.GetMembers("InterfaceEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_explicit_interface_implementation()
   {
      var src = @"
using System;

namespace Test;

public interface IMyInterface
{
   event EventHandler InterfaceEvent;
}

public class MyClass : IMyInterface
{
   event EventHandler IMyInterface.InterfaceEvent
   {
      add { }
      remove { }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("Test.IMyInterface.InterfaceEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_event_with_custom_add_remove_accessors()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   private EventHandler _backingField;

   public event EventHandler CustomEvent
   {
      add { _backingField += value; }
      remove { _backingField -= value; }
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("CustomEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText().ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("CustomEvent");
   }

   [Fact]
   public void Should_handle_event_in_nested_class()
   {
      var src = @"
using System;

namespace Test;

public class OuterClass
{
   public class InnerClass
   {
      public event EventHandler NestedEvent;
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.OuterClass+InnerClass");
      var eventSymbol = type.GetMembers("NestedEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_event_in_generic_class()
   {
      var src = @"
using System;

namespace Test;

public class GenericClass<T>
{
   public event EventHandler<T> GenericEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var eventSymbol = type.GetMembers("GenericEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_event_in_struct()
   {
      var src = @"
using System;

namespace Test;

public struct MyStruct
{
   public event EventHandler StructEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");
      var eventSymbol = type.GetMembers("StructEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_event_in_record()
   {
      var src = @"
using System;

namespace Test;

public record MyRecord
{
   public event EventHandler RecordEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecord");
      var eventSymbol = type.GetMembers("RecordEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_skip_generated_tree_with_designer_extension()
   {
      var userSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler UserEvent;
}
";
      var designerSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler DesignerEvent;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs");
      var designerTree = CSharpSyntaxTree.ParseText(designerSource, path: "MyClass.Designer.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, designerTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userEvent = type!.GetMembers("UserEvent").OfType<IEventSymbol>().Single();

      var location = userEvent.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".Designer.cs");
   }

   [Fact]
   public void Should_skip_generated_tree_with_generated_extension()
   {
      var userSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler UserEvent;
}
";
      var generatedSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler GeneratedEvent;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs");
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.generated.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userEvent = type!.GetMembers("UserEvent").OfType<IEventSymbol>().Single();

      var location = userEvent.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".generated.cs");
   }

   [Fact]
   public void Should_skip_generated_tree_with_g_i_cs_extension()
   {
      var userSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler UserEvent;
}
";
      var generatedSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler GeneratedEvent;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs");
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.i.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var userEvent = type!.GetMembers("UserEvent").OfType<IEventSymbol>().Single();

      var location = userEvent.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
      location.SourceTree?.FilePath.Should().NotContain(".g.i.cs");
   }

   [Fact]
   public void Should_handle_multiple_events_with_different_names_in_same_declaration()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler Alpha, Beta, Gamma, Delta;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var alphaEvent = type.GetMembers("Alpha").OfType<IEventSymbol>().Single();
      var betaEvent = type.GetMembers("Beta").OfType<IEventSymbol>().Single();
      var gammaEvent = type.GetMembers("Gamma").OfType<IEventSymbol>().Single();
      var deltaEvent = type.GetMembers("Delta").OfType<IEventSymbol>().Single();

      var alphaLocation = alphaEvent.GetEventLocation(CancellationToken.None);
      var betaLocation = betaEvent.GetEventLocation(CancellationToken.None);
      var gammaLocation = gammaEvent.GetEventLocation(CancellationToken.None);
      var deltaLocation = deltaEvent.GetEventLocation(CancellationToken.None);

      // All should have valid locations
      alphaLocation.Should().NotBe(Location.None);
      betaLocation.Should().NotBe(Location.None);
      gammaLocation.Should().NotBe(Location.None);
      deltaLocation.Should().NotBe(Location.None);

      // Verify each points to the correct identifier
      var sourceText = alphaLocation.SourceTree?.GetText().ToString();
      sourceText?.Substring(alphaLocation.SourceSpan.Start, alphaLocation.SourceSpan.Length).Should().Be("Alpha");
      sourceText?.Substring(betaLocation.SourceSpan.Start, betaLocation.SourceSpan.Length).Should().Be("Beta");
      sourceText?.Substring(gammaLocation.SourceSpan.Start, gammaLocation.SourceSpan.Length).Should().Be("Gamma");
      sourceText?.Substring(deltaLocation.SourceSpan.Start, deltaLocation.SourceSpan.Length).Should().Be("Delta");
   }

   [Fact]
   public void Should_handle_event_with_attributes()
   {
      var src = @"
using System;
using System.ComponentModel;

namespace Test;

public class MyClass
{
   [Browsable(false)]
   [Description(""This is a test event"")]
   public event EventHandler AttributedEvent;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("AttributedEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      var sourceText = location.SourceTree?.GetText().ToString();
      var locationText = sourceText?.Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      locationText.Should().Be("AttributedEvent");
   }

   [Fact]
   public void Should_handle_event_with_nullable_type()
   {
      var src = @"
#nullable enable
using System;

namespace Test;

public class MyClass
{
   public event EventHandler? NullableEvent;
}
";
      var compilation = CreateCompilation(src, nullableContextOptions: NullableContextOptions.Enable);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var eventSymbol = type.GetMembers("NullableEvent").OfType<IEventSymbol>().Single();

      var location = eventSymbol.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
   }

   [Fact]
   public void Should_prefer_non_generated_location_when_event_declared_in_multiple_trees()
   {
      var userSource = @"
using System;

namespace Test;

public partial class MyClass
{
   public event EventHandler SharedEvent;
}
";
      var generatedSource = @"
using System;

namespace Test;

public partial class MyClass
{
   // Different members but same type
   public event EventHandler GeneratedEvent;
}
";
      var userTree = CSharpSyntaxTree.ParseText(userSource, path: "MyClass.cs");
      var generatedTree = CSharpSyntaxTree.ParseText(generatedSource, path: "MyClass.g.cs");

      var compilation = CSharpCompilation.Create(
         "TestAssembly",
         [userTree, generatedTree],
         AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                  .Select(a => MetadataReference.CreateFromFile(a.Location)),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.MyClass");
      type.Should().NotBeNull();
      var sharedEvent = type!.GetMembers("SharedEvent").OfType<IEventSymbol>().Single();

      var location = sharedEvent.GetEventLocation(CancellationToken.None);

      location.Should().NotBe(Location.None);
      location.IsInSource.Should().BeTrue();
      location.SourceTree?.FilePath.Should().Be("MyClass.cs");
   }

   [Fact]
   public void Should_handle_case_sensitive_event_name_matching_in_EventFieldDeclarationSyntax()
   {
      var src = @"
using System;

namespace Test;

public class MyClass
{
   public event EventHandler myevent, MyEvent, MYEVENT;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var lowercaseEvent = type.GetMembers("myevent").OfType<IEventSymbol>().Single();
      var mixedcaseEvent = type.GetMembers("MyEvent").OfType<IEventSymbol>().Single();
      var uppercaseEvent = type.GetMembers("MYEVENT").OfType<IEventSymbol>().Single();

      var lowercaseLocation = lowercaseEvent.GetEventLocation(CancellationToken.None);
      var mixedcaseLocation = mixedcaseEvent.GetEventLocation(CancellationToken.None);
      var uppercaseLocation = uppercaseEvent.GetEventLocation(CancellationToken.None);

      // Verify each gets the correct location (case-sensitive)
      var sourceText = lowercaseLocation.SourceTree?.GetText().ToString();
      sourceText?.Substring(lowercaseLocation.SourceSpan.Start, lowercaseLocation.SourceSpan.Length).Should().Be("myevent");
      sourceText?.Substring(mixedcaseLocation.SourceSpan.Start, mixedcaseLocation.SourceSpan.Length).Should().Be("MyEvent");
      sourceText?.Substring(uppercaseLocation.SourceSpan.Start, uppercaseLocation.SourceSpan.Length).Should().Be("MYEVENT");
   }
}
