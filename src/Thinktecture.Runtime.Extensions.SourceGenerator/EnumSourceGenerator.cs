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
      private static readonly DiagnosticDescriptor _classMustBePartial = new("TTRE001", "Class must be partial", "The class '{0}' must be partial", nameof(EnumSourceGenerator), DiagnosticSeverity.Error, true);
      private static readonly DiagnosticDescriptor _fieldMustBeReadOnly = new("TTRE002", "Field must be read-only", "The field '{0}' of the class '{1}' must be read-only", nameof(EnumSourceGenerator), DiagnosticSeverity.Warning, true);

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
               var generatedCode = GenerateCode(enumDeclaration, context, model);
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
            context.ReportDiagnostic(Diagnostic.Create(_classMustBePartial,
                                                       enumDeclaration.ClassDeclarationSyntax.GetLocation(),
                                                       enumDeclaration.ClassDeclarationSyntax.Identifier));
            return false;
         }

         return true;
      }

      private static string GenerateCode(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model)
      {
         var classTypeInfo = model.GetDeclaredSymbol(enumDeclaration.ClassDeclarationSyntax);

         if (classTypeInfo is null)
            return String.Empty;

         var ns = classTypeInfo.ContainingNamespace.ToString();
         var keyType = GetKeyType(enumDeclaration.BaseType);
         var enumType = GetEnumType(enumDeclaration, model);
         var items = GetItems(enumDeclaration, context, model, classTypeInfo);

         var sb = new StringBuilder($@"
using System.Collections.Generic;

{(String.IsNullOrWhiteSpace(ns) ? null : $"namespace {ns}")}
{{
   [System.ComponentModel.TypeConverter(typeof(Thinktecture.EnumTypeConverter<{enumType}, {keyType}>))]
   partial class {enumDeclaration.ClassDeclarationSyntax.Identifier}
   {{
      private static IReadOnlyList<{enumDeclaration.ClassDeclarationSyntax.Identifier}> _items = new List<{enumDeclaration.ClassDeclarationSyntax.Identifier}>
      {{
");

         foreach (var item in items)
         {
            sb.Append("\t\t").Append(item.Declaration.Variables[0].Identifier.ToString()).AppendLine(",");
         }

         sb.Append($@"
      }}.AsReadOnly();

      public static System.Collections.Generic.IReadOnlyList<{enumDeclaration.ClassDeclarationSyntax.Identifier}> GetAll()
      {{
         return _items;
      }}
   }}
}}
");

         return sb.ToString();
      }

      private static IReadOnlyList<FieldDeclarationSyntax> GetItems(
         EnumDeclaration enumDeclaration,
         GeneratorExecutionContext context,
         SemanticModel model,
         INamedTypeSymbol classTypeInfo)
      {
         return enumDeclaration.ClassDeclarationSyntax.Members
                               .Select(m =>
                                       {
                                          if (m.IsStatic() && m.IsPublic() && m is FieldDeclarationSyntax fds)
                                          {
                                             var fieldTypeInfo = model.GetTypeInfo(fds.Declaration.Type).Type;

                                             if (SymbolEqualityComparer.Default.Equals(fieldTypeInfo, classTypeInfo))
                                             {
                                                if (m.IsReadOnly())
                                                   return fds;

                                                context.ReportDiagnostic(Diagnostic.Create(_fieldMustBeReadOnly,
                                                                                           fds.GetLocation(),
                                                                                           fds.Declaration.Variables[0].Identifier,
                                                                                           enumDeclaration.ClassDeclarationSyntax.Identifier));
                                             }
                                          }

                                          return null;
                                       })
                               .Where(fds => fds is not null)
                               .ToList()!;
      }

      private static string GetEnumType(EnumDeclaration enumDeclaration, SemanticModel model)
      {
         var typeInfo = model.GetTypeInfo(enumDeclaration.BaseType.TypeArgumentList.Arguments[0]).Type;

         if (typeInfo is null)
            return enumDeclaration.BaseType.TypeArgumentList.Arguments[0].ToString();

         return typeInfo.ToString();
      }

      private static string GetKeyType(GenericNameSyntax enumBaseType)
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
