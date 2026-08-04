using System.IO;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace Thinktecture.Runtime.Tests;

public class ThinktectureDestructuringPolicyTests
{
   private static (ILogger Logger, InMemorySink Sink) CreateLogger(
      TypesToRenderAsString renderAsString = TypesToRenderAsString.None)
   {
      var sink = new InMemorySink();
      var logger = new LoggerConfiguration()
                   .Destructure.UsingThinktectureRuntimeExtensions(renderAsString)
                   .WriteTo.Sink(sink)
                   .CreateLogger();

      return (logger, sink);
   }

   private static LogEventPropertyValue Capture(ILogger logger, InMemorySink sink, object value)
   {
      logger.Information("Value={@Value}", value);
      return sink.Events.Single().Properties["Value"];
   }

   // Renders a captured property the way a sink would, so "render as string" can be asserted on the output.
   private static string Render(LogEventPropertyValue value)
   {
      var writer = new StringWriter();
      value.Render(writer);
      return writer.ToString();
   }

   // --- Ad-hoc unions: default unwraps to Value ---

   [Fact]
   public void Should_unwrap_reference_union_to_scalar_value()
   {
      var (logger, sink) = CreateLogger();
      TestTypes.IntOrString value = 42;

      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(42);
   }

   [Fact]
   public void Should_unwrap_struct_union_to_scalar_value()
   {
      var (logger, sink) = CreateLogger();
      TestTypes.IntOrStringStruct value = "abc";

      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be("abc");
   }

   [Fact]
   public void Should_destructure_complex_union_value_as_structure()
   {
      var (logger, sink) = CreateLogger();
      var money = TestTypes.Money.Create(9.99m, "EUR");
      TestTypes.MoneyOrInt value = money;

      var structure = Capture(logger, sink, value).Should().BeOfType<StructureValue>().Subject;
      structure.Properties.Should().Contain(p => p.Name == "Amount");
      structure.Properties.Should().Contain(p => p.Name == "Currency");
   }

   [Fact]
   public void Should_compose_through_union_wrapping_a_value_object_to_its_key()
   {
      var (logger, sink) = CreateLogger();
      TestTypes.AmountOrText value = TestTypes.Amount.Create(7);

      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(7);
   }

   // --- Keyed types: default unwraps to key ---

   [Fact]
   public void Should_unwrap_simple_value_object_to_its_key()
   {
      var (logger, sink) = CreateLogger();
      var value = TestTypes.Amount.Create(7);

      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(7);
   }

   [Fact]
   public void Should_unwrap_keyed_smart_enum_to_its_key()
   {
      var (logger, sink) = CreateLogger();

      Capture(logger, sink, TestTypes.OrderStatus.Shipped).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(2);
   }

   // --- renderAsString flags: render as ToString() ---

   [Fact]
   public void Should_render_union_as_string_when_flag_set()
   {
      var (logger, sink) = CreateLogger(TypesToRenderAsString.AdHocUnions);
      TestTypes.IntOrString value = 42;

      // The scalar wraps the value itself (not a pre-stringified copy); Serilog renders it via ToString() at output time.
      var scalar = Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Subject;
      scalar.Value.Should().Be(value);
      Render(scalar).Should().Be(value.ToString());
   }

   [Fact]
   public void Should_render_smart_enum_as_string_when_flag_set()
   {
      var (logger, sink) = CreateLogger(TypesToRenderAsString.SmartEnums);

      var scalar = Capture(logger, sink, TestTypes.OrderStatus.Shipped).Should().BeOfType<ScalarValue>().Subject;
      scalar.Value.Should().Be(TestTypes.OrderStatus.Shipped);
      Render(scalar).Should().Be(TestTypes.OrderStatus.Shipped.ToString());
   }

   [Fact]
   public void Should_render_value_object_as_string_when_flag_set()
   {
      var (logger, sink) = CreateLogger(TypesToRenderAsString.ValueObjects);
      var value = TestTypes.Amount.Create(7);

      var scalar = Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Subject;
      scalar.Value.Should().Be(value);
      Render(scalar).Should().Be(value.ToString());
   }

   [Fact]
   public void Should_only_render_the_flagged_family_as_string()
   {
      // SmartEnums flagged, but a value object is still unwrapped to its key
      var (logger, sink) = CreateLogger(TypesToRenderAsString.SmartEnums);
      var value = TestTypes.Amount.Create(7);

      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(7);
   }

   // --- Negatives: declined -> Serilog default destructuring ---

   [Fact]
   public void Should_decline_complex_value_object()
   {
      var (logger, sink) = CreateLogger();
      var value = TestTypes.Money.Create(9.99m, "EUR");

      Capture(logger, sink, value).Should().BeOfType<StructureValue>();
   }

   [Fact]
   public void Should_decline_keyless_smart_enum()
   {
      var (logger, sink) = CreateLogger();

      // Declined -> Serilog default destructuring yields a StructureValue, not the key.
      Capture(logger, sink, TestTypes.Color.Red).Should().BeOfType<StructureValue>();
   }

   [Fact]
   public void Should_unwrap_union_with_renamed_Value_member_to_scalar_value()
   {
      var (logger, sink) = CreateLogger();
      TestTypes.RenamedValueUnion value = 42;

      // Destructuring reads the raw value through the GetValue metadata delegate, which the generator
      // rebinds to the renamed member, so the renamed union still unwraps to its scalar value.
      Capture(logger, sink, value).Should().BeOfType<ScalarValue>().Which.Value.Should().Be(42);
   }

   [Fact]
   public void Should_decline_regular_union()
   {
      var (logger, sink) = CreateLogger();

      Capture(logger, sink, new TestTypes.Shape.Circle()).Should().BeOfType<StructureValue>();
   }

   [Fact]
   public void Should_decline_plain_poco()
   {
      var (logger, sink) = CreateLogger();

      Capture(logger, sink, new { A = 1 }).Should().BeOfType<StructureValue>();
   }
}
