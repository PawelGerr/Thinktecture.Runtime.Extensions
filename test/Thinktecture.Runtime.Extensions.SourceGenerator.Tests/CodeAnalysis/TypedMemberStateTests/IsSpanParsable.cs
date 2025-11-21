using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateTests;

public class IsSpanParsable : CompilationTestBase
{
   [Fact]
   public void Int_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public int P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Int32);
      state.IsSpanParsable.Should().BeTrue("int implements ISpanParsable<int>");
   }

   [Fact]
   public void Decimal_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public decimal P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Decimal);
      state.IsSpanParsable.Should().BeTrue("decimal implements ISpanParsable<decimal>");
   }

   [Fact]
   public void Long_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public long P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Int64);
      state.IsSpanParsable.Should().BeTrue("long implements ISpanParsable<long>");
   }

   [Fact]
   public void Guid_should_be_span_parsable()
   {
      // Arrange
      var source = """
         using System;

         namespace N
         {
            public class C
            {
               public Guid P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeTrue("Guid implements ISpanParsable<Guid>");
   }

   [Fact]
   public void DateTime_should_be_span_parsable()
   {
      // Arrange
      var source = """
         using System;

         namespace N
         {
            public class C
            {
               public DateTime P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeTrue("DateTime implements ISpanParsable<DateTime>");
   }

   [Fact]
   public void DateTimeOffset_should_be_span_parsable()
   {
      // Arrange
      var source = """
         using System;

         namespace N
         {
            public class C
            {
               public DateTimeOffset P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeTrue("DateTimeOffset implements ISpanParsable<DateTimeOffset>");
   }

   [Fact]
   public void TimeSpan_should_be_span_parsable()
   {
      // Arrange
      var source = """
         using System;

         namespace N
         {
            public class C
            {
               public TimeSpan P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeTrue("TimeSpan implements ISpanParsable<TimeSpan>");
   }

   [Fact]
   public void String_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public string P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_String);
      state.IsSpanParsable.Should().BeTrue("string implements ISpanParsable<string> in .NET 8+");
   }

   [Fact]
   public void Custom_type_implementing_ISpanParsable_should_be_span_parsable()
   {
      // Arrange
      var source = """
         using System;

         namespace N
         {
            public struct CustomParsable : ISpanParsable<CustomParsable>
            {
               public static CustomParsable Parse(string s, IFormatProvider? provider) => default;
               public static bool TryParse(string? s, IFormatProvider? provider, out CustomParsable result)
               {
                  result = default;
                  return true;
               }
               public static CustomParsable Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => default;
               public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomParsable result)
               {
                  result = default;
                  return true;
               }
            }

            public class C
            {
               public CustomParsable P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeTrue("CustomParsable implements ISpanParsable<CustomParsable>");
   }

   [Fact]
   public void Custom_type_NOT_implementing_ISpanParsable_should_NOT_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public struct CustomType
            {
               public int Value { get; }
            }

            public class C
            {
               public CustomType P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeFalse("CustomType does not implement ISpanParsable<CustomType>");
   }

   [Fact]
   public void Nullable_int_should_NOT_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public int? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsSpanParsable.Should().BeFalse("Nullable<int> (int?) does not implement ISpanParsable<Nullable<int>>");
   }

   [Fact]
   public void Byte_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public byte P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Byte);
      state.IsSpanParsable.Should().BeTrue("byte implements ISpanParsable<byte>");
   }

   [Fact]
   public void Double_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public double P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Double);
      state.IsSpanParsable.Should().BeTrue("double implements ISpanParsable<double>");
   }

   [Fact]
   public void Float_should_be_span_parsable()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public float P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Single);
      state.IsSpanParsable.Should().BeTrue("float implements ISpanParsable<float>");
   }
}
