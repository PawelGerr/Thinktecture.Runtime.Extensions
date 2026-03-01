using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.Refactorings;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SwitchMapCompletionRefactoringProvider))]
public sealed class SwitchMapCompletionRefactoringProvider : CodeRefactoringProvider
{
   public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
   {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      if (root is null)
         return;

      var node = root.FindNode(context.Span);

      // Walk all ancestor invocations, not just the first — the cursor may be on a nested
      // invocation (e.g. Console.WriteLine()) inside a SwitchPartially() argument.
      foreach (var invocation in node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>())
      {
         // Fast bail: check method name syntactically
         if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            continue;

         var methodName = memberAccess.Name.Identifier.Text;

         if (!IsSwitchOrMapMethodName(methodName))
            continue;

         var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

         if (semanticModel is null)
            return;

         var methods = GetThinktectureSwitchMapMethods(semanticModel, invocation, context.CancellationToken);

         if (methods.IsEmpty)
            return;

         var existingNamedArgs = GetExistingNamedArgs(invocation);
         var positionalArgCount = GetPositionalArgCount(invocation);

         foreach (var method in methods)
         {
            // Check if all arguments are already provided
            var nonStateParamCount = method.Parameters.Length;

            if (existingNamedArgs.Length + positionalArgCount >= nonStateParamCount)
               continue;

            var title = GetCodeActionTitle(method);
            var capturedMethod = method;

            context.RegisterRefactoring(
               CodeAction.Create(
                  title: title,
                  createChangedDocument: ct => GenerateArgumentsAsync(context.Document, invocation, capturedMethod, existingNamedArgs, positionalArgCount, ct),
                  equivalenceKey: title));
         }

         // Found a matching Switch/Map invocation — no need to check further ancestors
         return;
      }
   }

   private static async Task<Document> GenerateArgumentsAsync(
      Document document,
      InvocationExpressionSyntax invocation,
      IMethodSymbol method,
      ImmutableArray<string> existingNamedArgs,
      int positionalArgCount,
      CancellationToken cancellationToken)
   {
      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

      if (root is null)
         return document;

      var newArguments = BuildArguments(method, existingNamedArgs, positionalArgCount);

      if (newArguments.Count == 0)
         return document;

      // Merge existing arguments with the new ones
      var allArguments = SyntaxFactory.SeparatedList(
         invocation.ArgumentList.Arguments.Concat(newArguments));

      var baseIndentation = GetIndentation(invocation);
      var newArgumentList = BuildFormattedArgumentList(allArguments, baseIndentation);
      var newInvocation = invocation.WithArgumentList(newArgumentList);
      var newRoot = root.ReplaceNode(invocation, newInvocation);

      return document.WithSyntaxRoot(newRoot);
   }

   private static ImmutableArray<string> GetExistingNamedArgs(InvocationExpressionSyntax invocation)
   {
      var args = invocation.ArgumentList.Arguments;

      if (args.Count == 0)
         return ImmutableArray<string>.Empty;

      var builder = ImmutableArray.CreateBuilder<string>(args.Count);

      foreach (var arg in args)
      {
         if (arg.NameColon is not null)
            builder.Add(arg.NameColon.Name.Identifier.ValueText);
      }

      return builder.ToImmutable();
   }

   private static int GetPositionalArgCount(InvocationExpressionSyntax invocation)
   {
      var count = 0;

      foreach (var arg in invocation.ArgumentList.Arguments)
      {
         if (arg.NameColon is null)
            count++;
      }

      return count;
   }

   private static SyntaxTriviaList GetIndentation(SyntaxNode node)
   {
      for (var current = node; current is not null; current = current.Parent)
      {
         var leadingTrivia = current.GetLeadingTrivia();

         foreach (var trivia in leadingTrivia)
         {
            if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
               return SyntaxFactory.TriviaList(trivia);
         }
      }

      return SyntaxFactory.TriviaList();
   }

   private static bool IsSwitchOrMapMethodName(string methodName)
   {
      return methodName is Constants.Methods.SWITCH
                or Constants.Methods.SWITCH_PARTIALLY
                or Constants.Methods.MAP
                or Constants.Methods.MAP_PARTIALLY;
   }

   private static ImmutableArray<IMethodSymbol> GetThinktectureSwitchMapMethods(
      SemanticModel semanticModel,
      InvocationExpressionSyntax invocation,
      CancellationToken cancellationToken)
   {
      var symbolInfo = semanticModel.GetSymbolInfo(invocation, cancellationToken);

      // SwitchPartially/MapPartially with no args may resolve directly (all params optional)
      if (symbolInfo.Symbol is IMethodSymbol resolvedMethod)
         return IsThinktectureSwitchMapMethod(resolvedMethod) ? [resolvedMethod] : [];

      // CS7036 case: multiple candidates
      if (symbolInfo.CandidateSymbols.IsDefaultOrEmpty)
         return [];

      var builder = ImmutableArray.CreateBuilder<IMethodSymbol>(symbolInfo.CandidateSymbols.Length);

      foreach (var candidate in symbolInfo.CandidateSymbols)
      {
         if (candidate is IMethodSymbol candidateMethod && IsThinktectureSwitchMapMethod(candidateMethod))
            builder.Add(candidateMethod);
      }

      return builder.DrainToImmutable();
   }

   private static bool IsStateOverload(IMethodSymbol method)
   {
      // State overloads have:
      // - Action variants: 1+ type params with void return (TState for Action)
      // - Func/Map variants: 2+ type params (TState + TResult)
      return method is { ReturnsVoid: true, TypeParameters.Length: > 0 }
                or { ReturnsVoid: false, TypeParameters.Length: > 1 };
   }

   private static string GetCodeActionTitle(IMethodSymbol method)
   {
      var methodName = method.Name;
      var isPartially = methodName is Constants.Methods.SWITCH_PARTIALLY or Constants.Methods.MAP_PARTIALLY;
      var isState = IsStateOverload(method);

      if (methodName is Constants.Methods.MAP or Constants.Methods.MAP_PARTIALLY)
      {
         if (isState)
            return isPartially ? "Generate MapPartially arguments (with state)" : "Generate Map arguments (with state)";

         return isPartially ? "Generate MapPartially arguments" : "Generate Map arguments";
      }

      if (method.ReturnsVoid)
      {
         if (isState)
            return isPartially ? "Generate SwitchPartially arguments (Action, with state)" : "Generate Switch arguments (Action, with state)";

         return isPartially ? "Generate SwitchPartially arguments (Action)" : "Generate Switch arguments (Action)";
      }

      if (isState)
         return isPartially ? "Generate SwitchPartially arguments (Func<TResult>, with state)" : "Generate Switch arguments (Func<TResult>, with state)";

      return isPartially ? "Generate SwitchPartially arguments (Func<TResult>)" : "Generate Switch arguments (Func<TResult>)";
   }

   private static SeparatedSyntaxList<ArgumentSyntax> BuildArguments(
      IMethodSymbol method,
      ImmutableArray<string> existingNamedArgs,
      int positionalArgCount = 0)
   {
      var arguments = new List<ArgumentSyntax>();

      foreach (var parameter in method.Parameters)
      {
         // Skip parameters already covered by positional arguments
         if (parameter.Ordinal < positionalArgCount)
            continue;

         // Skip if already provided by name
         if (existingNamedArgs.Contains(parameter.Name, StringComparer.Ordinal))
            continue;

         var expression = BuildArgumentExpression(parameter, method);

         if (expression is null)
            continue;

         var nameColon = SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(CreateIdentifier(parameter.Name)));
         arguments.Add(SyntaxFactory.Argument(nameColon, default, expression));
      }

      return SyntaxFactory.SeparatedList(arguments);
   }

   private static ArgumentListSyntax BuildFormattedArgumentList(
      SeparatedSyntaxList<ArgumentSyntax> arguments,
      SyntaxTriviaList baseIndentation)
   {
      if (arguments.Count <= 1)
         return SyntaxFactory.ArgumentList(arguments);

      // Multi-line: place each argument on its own line
      var formattedArgs = new List<SyntaxNodeOrToken>();
      var lineBreakAndIndent = SyntaxFactory.TriviaList(
         SyntaxFactory.ElasticCarriageReturnLineFeed,
         SyntaxFactory.Whitespace(baseIndentation.ToFullString() + "   "));

      for (var i = 0; i < arguments.Count; i++)
      {
         var arg = arguments[i].WithLeadingTrivia(lineBreakAndIndent);
         formattedArgs.Add(arg);

         if (i < arguments.Count - 1)
            formattedArgs.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
      }

      return SyntaxFactory.ArgumentList(
         SyntaxFactory.Token(SyntaxKind.OpenParenToken),
         SyntaxFactory.SeparatedList<ArgumentSyntax>(formattedArgs),
         SyntaxFactory.Token(SyntaxKind.CloseParenToken));
   }

   private static ExpressionSyntax BuildArgumentExpression(IParameterSymbol parameter, IMethodSymbol method)
   {
      var paramType = parameter.Type;

      // Unwrap nullable for optional delegate parameters (SwitchPartially)
      if (paramType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType)
         paramType = nullableType.TypeArguments[0];

      // State parameter (always first parameter in state overloads): generate identifier reference
      if (IsStateOverload(method) && parameter.Ordinal == 0)
      {
         return SyntaxFactory.IdentifierName(CreateIdentifier(parameter.Name));
      }

      if (paramType is INamedTypeSymbol namedType)
      {
         // Check for System.Action
         if (namedType.IsSystemAction())
            return BuildActionLambda(namedType);

         // Check for System.Func
         if (namedType.IsSystemFunc())
            return BuildFuncLambda(namedType);

         // Check for Thinktecture.Argument<T>
         if (namedType.IsThinktectureArgument())
            return SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
      }

      // Non-delegate parameter (Map's TResult values, MapPartially's @default)
      // For Map: default
      // For MapPartially @default: also default (it's required, of type TResult)
      return SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
   }

   private static ExpressionSyntax BuildActionLambda(INamedTypeSymbol actionType)
   {
      var staticModifier = SyntaxFactory.TokenList(
         SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space));

      if (actionType.TypeArguments.Length == 0)
      {
         // Action → static () => { }
         return SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.Block())
                             .WithModifiers(staticModifier)
                             .WithParameterList(SyntaxFactory.ParameterList());
      }

      if (actionType.TypeArguments.Length == 1)
      {
         // Action<T> → static x => { }
         return SyntaxFactory.SimpleLambdaExpression(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")),
                                SyntaxFactory.Block())
                             .WithModifiers(staticModifier);
      }

      // Action<T1, T2, ...> → static (x1, x2, ...) => { }
      return SyntaxFactory.ParenthesizedLambdaExpression(
                             SyntaxFactory.ParameterList(BuildMultipleParameters(actionType.TypeArguments.Length)),
                             SyntaxFactory.Block())
                          .WithModifiers(staticModifier);
   }

   private static ExpressionSyntax BuildFuncLambda(INamedTypeSymbol funcType)
   {
      var staticModifier = SyntaxFactory.TokenList(
         SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space));

      var throwExpression = SyntaxFactory.ThrowExpression(
         SyntaxFactory.ObjectCreationExpression(
                         SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("NotImplementedException")))
                      .WithArgumentList(SyntaxFactory.ArgumentList()));

      if (funcType.TypeArguments.Length == 1)
      {
         // Func<TResult> → static () => throw new System.NotImplementedException()
         return SyntaxFactory.ParenthesizedLambdaExpression(throwExpression)
                             .WithModifiers(staticModifier)
                             .WithParameterList(SyntaxFactory.ParameterList());
      }

      if (funcType.TypeArguments.Length == 2)
      {
         // Func<T, TResult> → static x => throw new System.NotImplementedException()
         return SyntaxFactory.SimpleLambdaExpression(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")),
                                throwExpression)
                             .WithModifiers(staticModifier);
      }

      // Func<T1, T2, ..., TResult> → static (x1, x2, ...) => throw new System.NotImplementedException()
      // TypeArguments.Length - 1 because the last type argument is TResult
      return SyntaxFactory.ParenthesizedLambdaExpression(
                             SyntaxFactory.ParameterList(BuildMultipleParameters(funcType.TypeArguments.Length - 1)),
                             throwExpression)
                          .WithModifiers(staticModifier);
   }

   private static SeparatedSyntaxList<ParameterSyntax> BuildMultipleParameters(int count)
   {
      var nodesAndTokens = new SyntaxNodeOrToken[count * 2 - 1];

      for (var i = 0; i < count; i++)
      {
         if (i > 0)
         {
            nodesAndTokens[i * 2 - 1] = SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                     .WithTrailingTrivia(SyntaxFactory.Space);
         }

         var name = i == 0 ? "state" : "x";
         nodesAndTokens[i * 2] = SyntaxFactory.Parameter(SyntaxFactory.Identifier(name));
      }

      return SyntaxFactory.SeparatedList<ParameterSyntax>(nodesAndTokens);
   }

   private static bool IsThinktectureSwitchMapMethod(IMethodSymbol method)
   {
      if (!IsSwitchOrMapMethodName(method.Name))
         return false;

      var containingType = method.ContainingType;

      return containingType.IsSmartEnum()
             || containingType.IsAnyUnionType();
   }

   private static SyntaxToken CreateIdentifier(string name)
   {
      // C# keywords used as parameter names need @ prefix (e.g., @default)
      if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None)
      {
         return SyntaxFactory.Identifier(
            default(SyntaxTriviaList),
            SyntaxKind.IdentifierToken,
            "@" + name,
            name,
            default(SyntaxTriviaList));
      }

      return SyntaxFactory.Identifier(name);
   }
}
