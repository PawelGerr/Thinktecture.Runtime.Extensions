using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

// Shared fakes and helpers for these tests
internal static class StringBuilderTestHelpers
{
   internal sealed class FakeTypeInfo : ITypeInformation
   {
      public string TypeFullyQualified { get; }
      public string TypeMinimallyQualified { get; }
      public string? Namespace { get; }
      public string Name { get; }
      public ImmutableArray<ContainingTypeState> ContainingTypes { get; }
      public int NumberOfGenerics { get; }
      public bool IsReferenceType { get; }
      public bool IsValueType { get; }
      public bool IsRecord { get; }
      public bool IsTypeParameter { get; }
      public NullableAnnotation NullableAnnotation { get; }
      public bool IsNullableStruct { get; }
      public bool IsEqualWithReferenceEquality { get; }
      public bool DisallowsDefaultValue { get; }

      public FakeTypeInfo(
         string typeFullyQualified,
         string typeMinimallyQualified,
         string? ns = null,
         string name = "T",
         ImmutableArray<ContainingTypeState>? containingTypes = null,
         int numberOfGenerics = 0,
         bool isReferenceType = true,
         bool isValueType = false,
         bool isRecord = false,
         bool isTypeParameter = false,
         NullableAnnotation nullableAnnotation = NullableAnnotation.NotAnnotated,
         bool isNullableStruct = false,
         bool isEqualWithReferenceEquality = false,
         bool disallowsDefaultValue = false)
      {
         TypeFullyQualified = typeFullyQualified;
         TypeMinimallyQualified = typeMinimallyQualified;
         Namespace = ns;
         Name = name;
         ContainingTypes = containingTypes ?? [];
         NumberOfGenerics = numberOfGenerics;
         IsReferenceType = isReferenceType;
         IsValueType = isValueType;
         IsRecord = isRecord;
         IsTypeParameter = isTypeParameter;
         NullableAnnotation = nullableAnnotation;
         IsNullableStruct = isNullableStruct;
         IsEqualWithReferenceEquality = isEqualWithReferenceEquality;
         DisallowsDefaultValue = disallowsDefaultValue;
      }
   }

   internal sealed class FakeMemberState : IMemberState
   {
      public ArgumentName ArgumentName { get; }
      public string Name { get; }
      public SpecialType SpecialType { get; }

      public string TypeFullyQualified { get; }
      public bool IsReferenceType { get; }
      public bool IsValueType { get; }
      public bool IsRecord { get; }
      public bool IsTypeParameter { get; }
      public NullableAnnotation NullableAnnotation { get; }
      public bool IsNullableStruct { get; }

      public FakeMemberState(
         string name,
         string typeFullyQualified,
         bool isReferenceType = true,
         bool isValueType = false,
         bool isRecord = false,
         bool isTypeParameter = false,
         NullableAnnotation nullableAnnotation = NullableAnnotation.NotAnnotated,
         bool isNullableStruct = false,
         SpecialType specialType = SpecialType.None)
      {
         Name = name;
         ArgumentName = ArgumentName.Create(name);
         TypeFullyQualified = typeFullyQualified;
         IsReferenceType = isReferenceType;
         IsValueType = isValueType;
         IsRecord = isRecord;
         IsTypeParameter = isTypeParameter;
         NullableAnnotation = nullableAnnotation;
         IsNullableStruct = isNullableStruct;
         SpecialType = specialType;
      }

      public bool Equals(IMemberState? other) => ReferenceEquals(this, other);
      public override bool Equals(object? obj) => ReferenceEquals(this, obj);
      public override int GetHashCode() => 0;
   }

   internal sealed class FakeNamespaceAndName : INamespaceAndName
   {
      public string? Namespace { get; }
      public string Name { get; }
      public ImmutableArray<ContainingTypeState> ContainingTypes { get; }
      public int NumberOfGenerics { get; }

      public FakeNamespaceAndName(string? ns, string name, ImmutableArray<ContainingTypeState>? containingTypes = null, int numberOfGenerics = 0)
      {
         Namespace = ns;
         Name = name;
         ContainingTypes = containingTypes ?? [];
         NumberOfGenerics = numberOfGenerics;
      }
   }

   internal sealed class FakeHasGenerics(params ImmutableArray<GenericTypeParameterState> parameters) : IHasGenerics
   {
      public ImmutableArray<GenericTypeParameterState> GenericParameters { get; } = parameters;
   }

   internal static ImmutableArray<InstanceMemberInfo> CreateInstanceMembers(params (string Type, string Name)[] props)
   {
      // Build a simple compilation with a single type and requested properties.
      var members = new List<InstanceMemberInfo>();

      var src = new StringBuilder();
      src.AppendLine("namespace Test;");
      src.AppendLine("public class C");
      src.AppendLine("{");

      foreach (var p in props)
      {
         // auto-property with only get is fine
         src.Append("   public ").Append(p.Type).Append(' ').Append(p.Name).Append(" { get; }").AppendLine();
      }

      src.AppendLine("}");

      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(src.ToString(), parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create("Members",
                                                 [syntaxTree],
                                                 references,
                                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      var type = compilation.GetTypeByMetadataName("Test.C");
      if (type is null)
         throw new InvalidOperationException("Type 'Test.C' not found.");

      var factory = TypedMemberStateFactory.Create(compilation);

      foreach (var p in props)
      {
         var prop = type.GetMembers().OfType<IPropertySymbol>().First(m => m.Name == p.Name);
         var info = InstanceMemberInfo.CreateOrNull(factory, prop, populateValueObjectMemberSettings: false, allowedCaptureSymbols: false)
                    ?? throw new InvalidOperationException("InstanceMemberInfo.CreateOrNull returned null.");
         members.Add(info);
      }

      return [..members];
   }
}
