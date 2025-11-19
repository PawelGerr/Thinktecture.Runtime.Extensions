using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThinktectureRuntimeExtensionsInternalUsageAnalyzer : DiagnosticAnalyzer
{
   /// <inheritdoc />
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [DiagnosticsDescriptors.InternalApiUsage];

   /// <inheritdoc />
   public override void Initialize(AnalysisContext context)
   {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(static c => CheckTypeDeclaration(c, (INamedTypeSymbol)c.Symbol), SymbolKind.NamedType);
      context.RegisterSymbolAction(static c => CheckMethodDeclaration(c, (IMethodSymbol)c.Symbol), SymbolKind.Method);
      context.RegisterSymbolAction(static c => CheckPropertyDeclaration(c, (IPropertySymbol)c.Symbol), SymbolKind.Property);
      context.RegisterSymbolAction(static c => CheckFieldDeclaration(c, (IFieldSymbol)c.Symbol), SymbolKind.Field);

      context.RegisterOperationAction(static c => CheckVariableDeclaration(c, (IVariableDeclarationOperation)c.Operation), OperationKind.VariableDeclaration);
      context.RegisterOperationAction(static c => CheckTypeOf(c, (ITypeOfOperation)c.Operation), OperationKind.TypeOf);
      context.RegisterOperationAction(static c => CheckTypeConversion(c, (IConversionOperation)c.Operation), OperationKind.Conversion);

      context.RegisterOperationAction(static c => CheckMemberAccess(c, ((IFieldReferenceOperation)c.Operation).Field), OperationKind.FieldReference);
      context.RegisterOperationAction(static c => CheckMemberAccess(c, ((IPropertyReferenceOperation)c.Operation).Property), OperationKind.PropertyReference);
      context.RegisterOperationAction(static c => CheckMemberAccess(c, ((IMethodReferenceOperation)c.Operation).Method), OperationKind.MethodReference);
      context.RegisterOperationAction(static c => CheckMemberAccess(c, ((IObjectCreationOperation)c.Operation).Constructor), OperationKind.ObjectCreation);
      context.RegisterOperationAction(static c => CheckMethodCall(c, (IInvocationOperation)c.Operation), OperationKind.Invocation);
   }

   private static void CheckTypeDeclaration(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      if (IsThinktectureModule(context))
         return;

      CheckBaseClass(context, type);
      CheckInterfaces(context, type);
   }

   private static void CheckBaseClass(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      if (type.BaseType is not ITypeSymbol baseClass
          || !IsInternalUsage(baseClass))
         return;

      ReportDiagnostic<TypeDeclarationSyntax>(context, type, n => n.Identifier.GetLocation(), baseClass);
   }

   private static void CheckInterfaces(SymbolAnalysisContext context, INamedTypeSymbol type)
   {
      if (type.Interfaces.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < type.Interfaces.Length; i++)
      {
         var @interface = type.Interfaces[i];

         if (!IsInternalUsage(@interface))
            continue;

         ReportDiagnostic<TypeDeclarationSyntax>(context, type, n => n.Identifier.GetLocation(), @interface);
      }
   }

   private static void CheckMethodDeclaration(SymbolAnalysisContext context, IMethodSymbol method)
   {
      if (method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet
          || IsThinktectureModule(context))
         return;

      if (IsInternalUsage(method.ReturnType))
         ReportDiagnostic<MethodDeclarationSyntax>(context, n => n.ReturnType.GetLocation(), method.ReturnType);

      if (method.Parameters.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < method.Parameters.Length; i++)
      {
         var parameter = method.Parameters[i];

         if (method.IsGenericMethod && parameter.Type is ITypeParameterSymbol { ConstraintTypes.IsDefaultOrEmpty: false } typeParameter)
         {
            for (var j = 0; j < typeParameter.ConstraintTypes.Length; j++)
            {
               var constraintType = typeParameter.ConstraintTypes[j];

               if (IsInternalUsage(constraintType))
                  ReportDiagnostic<MethodDeclarationSyntax>(context, n => n.Identifier.GetLocation(), constraintType);
            }
         }

         if (!IsInternalUsage(parameter.Type))
            continue;

         ReportDiagnostic<ParameterSyntax>(context, parameter, n => n.Type?.GetLocation(), parameter.Type);
      }
   }

   private static void CheckPropertyDeclaration(
      SymbolAnalysisContext context,
      IPropertySymbol property)
   {
      if (IsThinktectureModule(context))
         return;

      if (IsInternalUsage(property.Type))
         ReportDiagnostic<PropertyDeclarationSyntax>(context, n => n.Type.GetLocation(), property.Type);
   }

   private static void CheckFieldDeclaration(
      SymbolAnalysisContext context,
      IFieldSymbol field)
   {
      if (IsThinktectureModule(context))
         return;

      if (IsInternalUsage(field.Type))
         ReportDiagnostic<VariableDeclaratorSyntax>(context, n => (n.Parent as VariableDeclarationSyntax)?.Type.GetLocation(), field.Type);
   }

   private static void CheckVariableDeclaration(OperationAnalysisContext context, IVariableDeclarationOperation variableDeclaration)
   {
      if (variableDeclaration.Declarators.IsDefaultOrEmpty || IsThinktectureModule(context))
         return;

      for (var i = 0; i < variableDeclaration.Declarators.Length; i++)
      {
         var declarator = variableDeclaration.Declarators[i];

         if (!IsInternalUsage(declarator.Symbol.Type))
            continue;

         ReportDiagnostic<VariableDeclarationSyntax>(context, n => n.Type, declarator.Symbol.Type);
         return;
      }
   }

   private static void CheckTypeOf(OperationAnalysisContext context, ITypeOfOperation typeCheck)
   {
      if (IsThinktectureModule(context))
         return;

      if (IsInternalUsage(typeCheck.TypeOperand))
         ReportDiagnostic(context, typeCheck.Syntax.GetLocation(), typeCheck.TypeOperand);
   }

   private static void CheckTypeConversion(
      OperationAnalysisContext context,
      IConversionOperation conversion)
   {
      if (conversion.Operand.Type is null || IsThinktectureModule(context))
         return;

      if (IsInternalUsage(conversion.Operand.Type))
         ReportDiagnostic(context, conversion.Syntax.GetLocation(), conversion.Operand.Type);
   }

   private static void CheckMemberAccess(OperationAnalysisContext context, ISymbol? member)
   {
      if (member is null || IsThinktectureModule(context))
         return;

      if (IsInternalUsage(member.ContainingType))
         ReportDiagnostic(context, member.ContainingType);
   }

   private static void CheckMethodCall(OperationAnalysisContext context, IInvocationOperation methodCall)
   {
      if (IsThinktectureModule(context))
         return;

      CheckGenerics(context, methodCall);

      if (IsInternalUsage(methodCall.TargetMethod.ContainingType))
         ReportDiagnostic<InvocationExpressionSyntax>(context, n => n.Expression, methodCall.TargetMethod.ContainingType);
   }

   private static void CheckGenerics(OperationAnalysisContext context, IInvocationOperation methodCall)
   {
      if (methodCall.TargetMethod.TypeArguments.IsDefaultOrEmpty)
         return;

      for (var i = 0; i < methodCall.TargetMethod.TypeArguments.Length; i++)
      {
         var typeArg = methodCall.TargetMethod.TypeArguments[i];

         if (IsInternalUsage(typeArg))
            ReportDiagnostic<InvocationExpressionSyntax>(context, n => (n.Expression as GenericNameSyntax)?.TypeArgumentList.Arguments[i] ?? (SyntaxNode)n, typeArg);
      }
   }

   private static void ReportDiagnostic<T>(
      OperationAnalysisContext context,
      Func<T, SyntaxNode> syntaxNodeSelector,
      ITypeSymbol internalType)
      where T : SyntaxNode
   {
      var syntaxNode = context.Operation.Syntax is T node
                          ? syntaxNodeSelector(node)
                          : context.Operation.Syntax;

      ReportDiagnostic(context, syntaxNode.GetLocation(), internalType);
   }

   private static void ReportDiagnostic(
      OperationAnalysisContext context,
      ITypeSymbol internalType)
   {
      ReportDiagnostic(context, context.Operation.Syntax.GetLocation(), internalType);
   }

   private static void ReportDiagnostic(
      OperationAnalysisContext context,
      Location location,
      ITypeSymbol internalType)
   {
      var diagnostic = Diagnostic.Create(
         DiagnosticsDescriptors.InternalApiUsage,
         location,
         internalType.ToDisplayString());

      context.ReportDiagnostic(diagnostic);
   }

   private static void ReportDiagnostic<T>(
      SymbolAnalysisContext context,
      Func<T, Location?> locationSelector,
      ITypeSymbol internalType)
      where T : SyntaxNode
   {
      ReportDiagnostic(context, GetLocation(context, locationSelector), internalType);
   }

   private static void ReportDiagnostic<T>(
      SymbolAnalysisContext context,
      ISymbol symbol,
      Func<T, Location?> locationSelector,
      ITypeSymbol internalType)
      where T : SyntaxNode
   {
      if (symbol.DeclaringSyntaxReferences.IsDefaultOrEmpty)
      {
         ReportDiagnostic(context, Location.None, internalType);
      }
      else
      {
         foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
         {
            ReportDiagnostic(context, syntaxReference, locationSelector, internalType);
         }
      }
   }

   private static void ReportDiagnostic<T>(
      SymbolAnalysisContext context,
      SyntaxReference syntaxReferences,
      Func<T, Location?> locationSelector,
      ITypeSymbol internalType)
      where T : SyntaxNode
   {
      var location = GetLocation(syntaxReferences, locationSelector);
      ReportDiagnostic(context, location, internalType);
   }

   private static void ReportDiagnostic(
      SymbolAnalysisContext context,
      Location location,
      ITypeSymbol internalType)
   {
      var diagnostic = Diagnostic.Create(
         DiagnosticsDescriptors.InternalApiUsage,
         location,
         internalType.ToDisplayString());

      context.ReportDiagnostic(diagnostic);
   }

   private static Location GetLocation<T>(
      SymbolAnalysisContext context,
      Func<T, Location?> locationSelector)
      where T : SyntaxNode
   {
      if (context.Symbol.DeclaringSyntaxReferences.IsDefaultOrEmpty)
         return Location.None;

      foreach (var syntaxReference in context.Symbol.DeclaringSyntaxReferences)
      {
         var syntaxNode = syntaxReference.GetSyntax();

         if (syntaxNode is T node)
            return locationSelector(node) ?? node.GetLocation();
      }

      return context.Symbol.DeclaringSyntaxReferences[0].GetSyntax().GetLocation();
   }

   private static Location GetLocation<T>(
      SyntaxReference syntaxReference,
      Func<T, Location?> locationSelector)
      where T : SyntaxNode
   {
      var syntaxNode = syntaxReference.GetSyntax();

      if (syntaxNode is T node)
         return locationSelector(node) ?? node.GetLocation();

      return syntaxNode.GetLocation();
   }

   private static bool IsInternalUsage(ITypeSymbol? symbol)
   {
      return symbol is not null
             && (IsInInternalNamespace(symbol) || HasInternalAttribute(symbol));
   }

   private static bool HasInternalAttribute(ISymbol symbol)
   {
      foreach (var attribute in symbol.GetAttributes())
      {
         if (attribute.AttributeClass is { Name: "ThinktectureRuntimeExtensionInternalAttribute", ContainingNamespace: { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true } })
            return true;
      }

      return false;
   }

   private static bool IsThinktectureModule(OperationAnalysisContext context)
   {
      return IsThinktectureModule(context.Compilation.SourceModule);
   }

   private static bool IsThinktectureModule(SymbolAnalysisContext context)
   {
      return IsThinktectureModule(context.Compilation.SourceModule);
   }

   private static bool IsThinktectureModule(IModuleSymbol? containingModule)
   {
      return containingModule?.Name.StartsWith("Thinktecture.Runtime.Extensions", StringComparison.Ordinal) == true;
   }

   private static bool IsInInternalNamespace(ISymbol symbol)
   {
      if (!IsThinktectureModule(symbol.ContainingModule))
         return false;

      var ns = symbol.ContainingNamespace;

      while (ns is not null)
      {
         if (ns.IsGlobalNamespace)
            return false;

         if (ns.Name == "Internal")
            return true;

         ns = ns.ContainingNamespace;
      }

      return false;
   }
}
