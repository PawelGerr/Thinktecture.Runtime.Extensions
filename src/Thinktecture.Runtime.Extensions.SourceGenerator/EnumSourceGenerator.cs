using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   /// <summary>
   /// Source generator for <see cref="Enum{TEnum}"/> and <see cref="Enum{TEnum,TValue}"/>.
   /// </summary>
   [Generator]
   public class EnumSourceGenerator : ISourceGenerator
   {
      private const string _ERROR_ENUM_MUST_BE_PARTIAL = "TTREGEN001";

      /// <inheritdoc />
      public void Initialize(GeneratorInitializationContext context)
      {
         context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
      }

      /// <inheritdoc />
      public void Execute(GeneratorExecutionContext context)
      {
         var receiver = (EnumSyntaxReceiver)(context.SyntaxReceiver ?? throw new Exception($"Syntax receiver must be of type '{nameof(EnumSyntaxReceiver)}' but found '{context.SyntaxReceiver?.GetType().Name}'."));

         foreach (var enumDeclaration in receiver.Enums)
         {
            var model = context.Compilation.GetSemanticModel(enumDeclaration.ClassDeclarationSyntax.SyntaxTree, true);

            if (IsValid(enumDeclaration, context, model))
            {
               var generatedCode = GenerateCode(enumDeclaration, model);
               context.AddSource($"{enumDeclaration.ClassDeclarationSyntax.Identifier}_Generated.cs", generatedCode);
            }
         }
      }

      private static bool IsValid(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         var typeInfo = model.GetTypeInfo(enumDeclaration.BaseType).Type;

         if (typeInfo is null)
            return false;

         if (typeInfo.ContainingNamespace.Name != "Thinktecture")
            return false;

         if (!enumDeclaration.ClassDeclarationSyntax.IsPartial())
         {
            context.ReportDiagnostic(Diagnostic.Create(_ERROR_ENUM_MUST_BE_PARTIAL,
                                                       nameof(EnumSourceGenerator),
                                                       $"The class '{enumDeclaration.ClassDeclarationSyntax.Identifier}' must be partial.",
                                                       DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false,
                                                       location: enumDeclaration.ClassDeclarationSyntax.GetLocation()));
            return false;
         }

         return true;
      }

      private static string GenerateCode(EnumDeclaration enumDeclaration, SemanticModel model)
      {
         var ns = enumDeclaration.ClassDeclarationSyntax.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString();
         var keyType = GetKeyType(enumDeclaration.BaseType, model);
         var enumType = GetEnumType(enumDeclaration, model);
         var sb = new StringBuilder($@"
{(String.IsNullOrWhiteSpace(ns) ? null : $"namespace {ns}")}
{{
   [System.ComponentModel.TypeConverter(typeof(Thinktecture.EnumTypeConverter<{enumType}, {keyType}>))]
   partial class {enumDeclaration.ClassDeclarationSyntax.Identifier}
   {{

   }}
}}
");

         return sb.ToString();
      }

      private static string GetEnumType(EnumDeclaration enumDeclaration, SemanticModel model)
      {
         var typeInfo = model.GetTypeInfo(enumDeclaration.BaseType.TypeArgumentList.Arguments[0]).Type;

         if (typeInfo is null)
            return enumDeclaration.BaseType.TypeArgumentList.Arguments[0].ToString();

         return typeInfo.ToString();
      }

      private static string GetKeyType(GenericNameSyntax enumBaseType, SemanticModel model)
      {
         if (enumBaseType.TypeArgumentList.Arguments.Count == 1)
            return "string";

         return enumBaseType.TypeArgumentList.Arguments[1].ToString();
      }
   }

   internal class EnumSyntaxReceiver : ISyntaxReceiver
   {
      public List<EnumDeclaration> Enums { get; }

      public EnumSyntaxReceiver()
      {
         Enums = new List<EnumDeclaration>();
      }

      /// <inheritdoc />
      public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
      {
         if (syntaxNode is ClassDeclarationSyntax cds)
         {
            if (cds.IsAbstract())
               return;

            if (IsEnum(cds, out var enumDeclaration))
               Enums.Add(enumDeclaration);
         }
      }

      private static bool IsEnum(
         ClassDeclarationSyntax cds,
         [MaybeNullWhen(false)] out EnumDeclaration enumDeclaration)
      {
         if (cds.BaseList?.Types.Count > 0)
         {
            foreach (var type in cds.BaseList.Types)
            {
               if (type.Type is GenericNameSyntax genNameSyntax)
               {
                  if (genNameSyntax.Identifier.Text == "Enum")
                  {
                     switch (genNameSyntax.TypeArgumentList.Arguments.Count)
                     {
                        case 1:
                        case 2:
                           enumDeclaration = new EnumDeclaration(cds, genNameSyntax);
                           return true;
                     }
                  }
               }
            }
         }

         enumDeclaration = null;
         return false;
      }
   }

   internal class EnumDeclaration
   {
      public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
      public GenericNameSyntax BaseType { get; }

      public EnumDeclaration(
         ClassDeclarationSyntax cds,
         GenericNameSyntax baseType)
      {
         ClassDeclarationSyntax = cds ?? throw new ArgumentNullException(nameof(cds));
         BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
      }
   }
}
