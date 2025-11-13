using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateFactoryTests;

public class Create : CompilationTestBase
{
   // Built-in primitive value types

   [Fact]
   public void NonNullable_builtin_value_type_int_should_use_cached_notnullable_state()
   {
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

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.SpecialType.Should().Be(SpecialType.System_Int32);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Nullable_builtin_value_type_int_should_use_cached_nullable_state()
   {
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

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.OriginalDefinition.SpecialType.Should().Be(SpecialType.System_Nullable_T);
      state.Should().BeOfType<CachedTypedMemberState>("Nullable<int> must route to cached int and pick Nullable variant");
      state.IsNullableStruct.Should().BeTrue();
   }

   // Built-in reference types

   [Fact]
   public void NonNullable_reference_type_string_should_use_cached_notnullable_state()
   {
      var source = """
         namespace N
         {
            public class C
            {
               public string P { get; } = "";
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.SpecialType.Should().Be(SpecialType.System_String);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Nullable_reference_type_string_should_use_cached_nullable_state()
   {
      var source = """
         namespace N
         {
            public class C
            {
               public string? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.SpecialType.Should().Be(SpecialType.System_String);
      p.Type.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   // Special types registered via token lookup (System.Runtime / System.Private.CoreLib)

   [Fact]
   public void NonNullable_struct_Guid_should_use_cached_notnullable_state()
   {
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

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Nullable_struct_Guid_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Guid? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.OriginalDefinition.SpecialType.Should().Be(SpecialType.System_Nullable_T);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_struct_DateTime_should_use_cached_notnullable_state()
   {
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

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.SpecialType.Should().Be(SpecialType.System_DateTime);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Nullable_struct_DateTime_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public DateTime? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      p.Type.OriginalDefinition.SpecialType.Should().Be(SpecialType.System_Nullable_T);
      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void Nullable_struct_TimeSpan_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public TimeSpan? P { get; }
            }
         }
         """;
      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();
      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void Nullable_struct_DateOnly_should_use_cached_nullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public DateOnly? P { get; }
            }
         }
         """;
      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();
      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      // On some TFMs DateOnly may not be present; but CompilationTestBase uses host framework,
      // which should include DateOnly on modern SDKs. If present, we expect caching.
      if (p.Type is INamedTypeSymbol named && named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" && named.TypeArguments[0].ToDisplayString() == "System.DateOnly")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeTrue();
      }
   }

   [Fact]
   public void Nullable_struct_TimeOnly_should_use_cached_nullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public TimeOnly? P { get; }
            }
         }
         """;
      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();
      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type is INamedTypeSymbol named && named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" && named.TypeArguments[0].ToDisplayString() == "System.TimeOnly")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeTrue();
      }
   }

   // Non-cached types

   [Fact]
   public void Custom_struct_nonnullable_should_not_use_cache()
   {
      var source = """
         namespace N
         {
            public struct S { }
            public class C
            {
               public S P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<TypedMemberState>("custom struct is not pre-cached");
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Custom_struct_nullable_should_not_use_cache_but_be_nullable_struct()
   {
      var source = """
         namespace N
         {
            public struct S { }
            public class C
            {
               public S? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<TypedMemberState>("custom struct Nullable<S> not pre-cached");
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void Array_type_should_not_use_cache()
   {
      var source = """
         namespace N
         {
            public class C
            {
               public int[] P { get; } = System.Array.Empty<int>();
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      // Array types have no ContainingModule; factory must fall back to non-cached path
      p.Type.TypeKind.Should().Be(TypeKind.Array);
      state.Should().BeOfType<TypedMemberState>();
   }

   [Fact]
   public void Type_parameter_with_struct_constraint_nullable_should_not_use_cache_but_be_nullable_struct()
   {
      var source = """
         namespace N
         {
            public class G<T> where T : struct
            {
               public T? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var g = GetTypeSymbol(compilation, "N.G`1");
      var p = g.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      // Nullable<TParam> where TParam : struct -> underlying is a type parameter (no module/token), so no cache
      state.Should().BeOfType<TypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void Custom_reference_type_nonnullable_should_not_use_cache()
   {
      var source = """
         namespace N
         {
            public class R { }
            public class C
            {
               public R P { get; } = new R();
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<TypedMemberState>("non-special reference types are not pre-cached");
   }

   [Fact]
   public void Type_parameter_with_class_constraint_nonnullable_should_not_use_cache()
   {
      var source = """
         namespace N
         {
            public class G<T> where T : class
            {
               public T P { get; } = null!;
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var g = GetTypeSymbol(compilation, "N.G`1");
      var p = g.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<TypedMemberState>("type parameter with class constraint is not pre-cached");
   }

   // Newly cached BCL types

   [Fact]
   public void NonNullable_struct_DateTimeOffset_should_use_cached_notnullable_state()
   {
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

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Nullable_struct_DateTimeOffset_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public DateTimeOffset? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_reference_type_Version_should_use_cached_notnullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Version P { get; } = new Version(1, 0);
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Nullable_reference_type_Version_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Version? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_struct_Half_should_use_cached_notnullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Half P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // If Half is available on this TFM, we expect caching
      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type.ToDisplayString() == "System.Half")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeFalse();
      }
   }

   [Fact]
   public void Nullable_struct_Half_should_use_cached_nullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Half? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type is INamedTypeSymbol named && named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" && named.TypeArguments[0].ToDisplayString() == "System.Half")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeTrue();
      }
   }

   [Fact]
   public void NonNullable_struct_Int128_should_use_cached_notnullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Int128 P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type.ToDisplayString() == "System.Int128")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeFalse();
      }
   }

   [Fact]
   public void Nullable_struct_Int128_should_use_cached_nullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Int128? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type is INamedTypeSymbol named && named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" && named.TypeArguments[0].ToDisplayString() == "System.Int128")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeTrue();
      }
   }

   [Fact]
   public void NonNullable_struct_UInt128_should_use_cached_notnullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public UInt128 P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type.ToDisplayString() == "System.UInt128")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeFalse();
      }
   }

   [Fact]
   public void Nullable_struct_UInt128_should_use_cached_nullable_state_if_available()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public UInt128? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      if (p.Type is INamedTypeSymbol named && named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" && named.TypeArguments[0].ToDisplayString() == "System.UInt128")
      {
         state.Should().BeOfType<CachedTypedMemberState>();
         state.IsNullableStruct.Should().BeTrue();
      }
   }

   [Fact]
   public void NonNullable_reference_type_Uri_should_use_cached_notnullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Uri P { get; } = new Uri("https://example.com");
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Uri).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Nullable_reference_type_Uri_should_use_cached_nullable_state()
   {
      var source = """
         using System;
         namespace N
         {
            public class C
            {
               public Uri? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Uri).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_struct_BigInteger_should_use_cached_notnullable_state()
   {
      var source = """
         using System.Numerics;
         namespace N
         {
            public class C
            {
               public BigInteger P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Numerics.BigInteger).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeFalse();
   }

   [Fact]
   public void Nullable_struct_BigInteger_should_use_cached_nullable_state()
   {
      var source = """
         using System.Numerics;
         namespace N
         {
            public class C
            {
               public BigInteger? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Numerics.BigInteger).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_reference_type_IPAddress_should_use_cached_notnullable_state()
   {
      var source = """
         using System.Net;
         namespace N
         {
            public class C
            {
               public IPAddress P { get; } = IPAddress.Loopback;
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Net.IPAddress).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Nullable_reference_type_IPAddress_should_use_cached_nullable_state()
   {
      var source = """
         using System.Net;
         namespace N
         {
            public class C
            {
               public IPAddress? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Net.IPAddress).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void NonNullable_reference_type_IPEndPoint_should_use_cached_notnullable_state()
   {
      var source = """
         using System.Net;
         namespace N
         {
            public class C
            {
               public IPEndPoint P { get; } = new IPEndPoint(IPAddress.Loopback, 80);
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Net.IPAddress).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Nullable_reference_type_IPEndPoint_should_use_cached_nullable_state()
   {
      var source = """
         using System.Net;
         namespace N
         {
            public class C
            {
               public IPEndPoint? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Net.IPAddress).Assembly.Location]);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      var factory = TypedMemberStateFactory.Create(compilation);
      var state = factory.Create(p.Type);

      state.Should().BeOfType<CachedTypedMemberState>();
      state.IsReferenceType.Should().BeTrue();
   }
}
