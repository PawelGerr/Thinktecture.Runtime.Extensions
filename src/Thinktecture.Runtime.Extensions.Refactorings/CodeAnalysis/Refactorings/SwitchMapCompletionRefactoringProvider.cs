using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.Refactorings;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SwitchMapCompletionRefactoringProvider))]
public sealed class SwitchMapCompletionRefactoringProvider : CodeRefactoringProvider
{
   private static readonly string[] _valueParameterNameCandidates = ["x", "value", "v", "arg", "item"];

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

         // This invocation shares the name Switch/Map but is not a Thinktecture method (e.g. a
         // foreign Map inside a lambda argument). Keep walking the ancestors so the enclosing
         // Thinktecture invocation still gets the refactoring.
         if (methods.IsEmpty)
            continue;

         var existingNamedArgs = GetExistingNamedArgs(invocation);
         var positionalOrdinals = GetPositionalArgOrdinals(invocation);

         foreach (var method in methods)
         {
            // Check if all arguments are already provided
            var nonStateParamCount = method.Parameters.Length;

            if (existingNamedArgs.Length + positionalOrdinals.Length >= nonStateParamCount)
               continue;

            var title = GetCodeActionTitle(method);
            var capturedMethod = method;

            context.RegisterRefactoring(
               CodeAction.Create(
                  title: title,
                  createChangedDocument: ct => GenerateArgumentsAsync(context.Document, invocation, capturedMethod, existingNamedArgs, positionalOrdinals, ct),
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
      ImmutableArray<int> positionalOrdinals,
      CancellationToken cancellationToken)
   {
      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

      if (root is null)
         return document;

      var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
      var reservedNames = GetReservedLambdaParameterNames(semanticModel, invocation);

      var newArguments = BuildArguments(method, existingNamedArgs, positionalOrdinals, reservedNames);

      if (newArguments.Count == 0)
         return document;

      var baseIndentation = GetIndentation(invocation);
      var eol = DetectEndOfLine(root);
      var newArgumentList = BuildFormattedArgumentList(invocation.ArgumentList, newArguments, baseIndentation, eol);
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

   private static ImmutableArray<int> GetPositionalArgOrdinals(InvocationExpressionSyntax invocation)
   {
      var args = invocation.ArgumentList.Arguments;
      var builder = ImmutableArray.CreateBuilder<int>(args.Count);

      for (var i = 0; i < args.Count; i++)
      {
         // A positional argument binds the parameter whose ordinal equals the argument's position
         // in the argument list. This holds even when a non-trailing named argument (C# 7.2)
         // precedes it, because such a named argument must sit at its own correct position.
         if (args[i].NameColon is null)
            builder.Add(i);
      }

      return builder.DrainToImmutable();
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

   private static string DetectEndOfLine(SyntaxNode root)
   {
      foreach (var trivia in root.DescendantTrivia())
      {
         if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
            return trivia.ToFullString();
      }

      return "\n";
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
      ImmutableArray<int> positionalOrdinals,
      HashSet<string> reservedNames)
   {
      var arguments = new List<ArgumentSyntax>();

      foreach (var parameter in method.Parameters)
      {
         // Skip parameters already bound by a positional argument. Comparing against the actual set
         // of bound ordinals (instead of a leading count) is correct even when a non-trailing named
         // argument shifts the positional argument to a later ordinal.
         if (positionalOrdinals.Contains(parameter.Ordinal))
            continue;

         // Skip if already provided by name
         if (existingNamedArgs.Contains(parameter.Name, StringComparer.Ordinal))
            continue;

         var expression = BuildArgumentExpression(parameter, method, reservedNames);

         if (expression is null)
            continue;

         var nameColon = SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(CreateIdentifier(parameter.Name)));
         arguments.Add(SyntaxFactory.Argument(nameColon, default, expression));
      }

      return SyntaxFactory.SeparatedList(arguments);
   }

   private static ArgumentListSyntax BuildFormattedArgumentList(
      ArgumentListSyntax existingArgumentList,
      SeparatedSyntaxList<ArgumentSyntax> newArguments,
      SyntaxTriviaList baseIndentation,
      string eol)
   {
      var existingArguments = existingArgumentList.Arguments;

      // A single argument in total stays on one line. This is the only shape that can reach the
      // branch, because GenerateArgumentsAsync returns early when nothing was generated. Therefore
      // no existing argument can be lost here.
      if (existingArguments.Count == 0 && newArguments.Count == 1)
         return existingArgumentList.WithArguments(newArguments);

      // Multi-line: place each argument on its own line
      var indentation = SyntaxFactory.Whitespace(baseIndentation.ToFullString() + "   ");
      var lineBreakAndIndent = SyntaxFactory.TriviaList(SyntaxFactory.ElasticEndOfLine(eol), indentation);
      var formattedArgs = new List<SyntaxNodeOrToken>(((existingArguments.Count + newArguments.Count) * 2) - 1);

      for (var i = 0; i < existingArguments.Count; i++)
      {
         var existingArgument = existingArguments[i];

         // The separator in front of the current argument is the original comma that has to be
         // recreated. Its trivia is carried over so that a comment behind it survives.
         AddSeparatingComma(formattedArgs, i > 0 && i - 1 < existingArguments.SeparatorCount ? existingArguments.GetSeparator(i - 1) : default);

         // Regenerate the line break and indentation, but keep any comment or directive the user
         // attached to the existing argument, so applying the refactoring does not silently delete it.
         var leadingTrivia = BuildLeadingTrivia(lineBreakAndIndent, indentation, existingArgument.GetLeadingTrivia());
         formattedArgs.Add(existingArgument.WithLeadingTrivia(leadingTrivia));
      }

      foreach (var newArgument in newArguments)
      {
         AddSeparatingComma(formattedArgs, default);
         formattedArgs.Add(newArgument.WithLeadingTrivia(lineBreakAndIndent));
      }

      var openParenToken = existingArgumentList.OpenParenToken;
      var closeParenToken = existingArgumentList.CloseParenToken;
      var preservedCloseParenTrivia = KeepPreservedTrivia(closeParenToken.LeadingTrivia);

      // The original paren tokens are reused so that a comment or directive attached to them
      // survives. Their own whitespace and line breaks are dropped, because the argument formatting
      // regenerates them.
      return existingArgumentList
             .WithOpenParenToken(openParenToken.WithTrailingTrivia(KeepPreservedTrivia(openParenToken.TrailingTrivia)))
             .WithArguments(SyntaxFactory.SeparatedList<ArgumentSyntax>(formattedArgs))
             .WithCloseParenToken(closeParenToken.WithLeadingTrivia(
                                     preservedCloseParenTrivia.Count == 0
                                        ? SyntaxFactory.TriviaList()
                                        : BuildLeadingTrivia(lineBreakAndIndent, indentation, preservedCloseParenTrivia)));
   }

   private static void AddSeparatingComma(List<SyntaxNodeOrToken> formattedArgs, SyntaxToken originalSeparator)
   {
      if (formattedArgs.Count == 0)
         return;

      var lastIndex = formattedArgs.Count - 1;
      var triviaBehindComma = SyntaxFactory.TriviaList();

      // Any trailing trivia on the previous argument would push the comma onto its own line. A comment
      // moves behind the comma instead; whitespace and line breaks are dropped, because the following
      // argument brings its own ones.
      if (formattedArgs[lastIndex].AsNode() is { } previousArgument)
      {
         triviaBehindComma = GetMoveableTrivia(previousArgument.GetTrailingTrivia());
         formattedArgs[lastIndex] = previousArgument.WithTrailingTrivia(SyntaxFactory.TriviaList());
      }

      // A comment written behind the original comma (for example "argument, // note") belongs to the
      // separator token. The separators are recreated here, so the comment has to be carried over.
      triviaBehindComma = triviaBehindComma.AddRange(GetMoveableTrivia(originalSeparator.LeadingTrivia))
                                           .AddRange(GetMoveableTrivia(originalSeparator.TrailingTrivia));

      var comma = SyntaxFactory.Token(SyntaxKind.CommaToken);

      formattedArgs.Add(triviaBehindComma.Count == 0 ? comma : comma.WithTrailingTrivia(triviaBehindComma));
   }

   private static SyntaxTriviaList GetMoveableTrivia(SyntaxTriviaList trivia)
   {
      // Move nothing when the trivia is plain formatting, because the arguments regenerate their own
      // line breaks and indentation. Otherwise keep the whitespace between the code and the comment
      // and drop the line breaks, because the following argument brings its own one.
      if (!trivia.Any(IsPreservedTrivia))
         return SyntaxFactory.TriviaList();

      return SyntaxFactory.TriviaList(trivia.Where(t => !t.IsKind(SyntaxKind.EndOfLineTrivia)));
   }

   private static SyntaxTriviaList BuildLeadingTrivia(
      SyntaxTriviaList lineBreakAndIndent,
      SyntaxTrivia indentation,
      SyntaxTriviaList originalLeadingTrivia)
   {
      var result = SyntaxFactory.TriviaList();
      var needsLineBreak = true;

      foreach (var trivia in originalLeadingTrivia)
      {
         if (!IsPreservedTrivia(trivia))
            continue;

         result = needsLineBreak
                     ? result.AddRange(lineBreakAndIndent).Add(trivia)
                     : result.Add(indentation).Add(trivia);

         // Directives and documentation comments end with their own line break; adding another one
         // would insert a blank line.
         needsLineBreak = !trivia.ToFullString().EndsWith("\n", StringComparison.Ordinal);
      }

      // Without any preserved trivia this is exactly the line break and indentation.
      return needsLineBreak ? result.AddRange(lineBreakAndIndent) : result.Add(indentation);
   }

   private static SyntaxTriviaList KeepPreservedTrivia(SyntaxTriviaList trivia)
   {
      return SyntaxFactory.TriviaList(trivia.Where(IsPreservedTrivia));
   }

   private static bool IsPreservedTrivia(SyntaxTrivia trivia)
   {
      // Keep everything that is not plain formatting: comments, documentation comments, preprocessor
      // directives and pragmas. Dropping a directive can break the compilation of the user's file,
      // for example when an "#if" loses its "#endif".
      return !trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia);
   }

   private static ExpressionSyntax BuildArgumentExpression(IParameterSymbol parameter, IMethodSymbol method, HashSet<string> reservedNames)
   {
      var paramType = parameter.Type;

      // Unwrap nullable for optional delegate parameters (SwitchPartially)
      if (paramType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType)
         paramType = nullableType.TypeArguments[0];

      var isStateOverload = IsStateOverload(method);

      // State parameter (always first parameter in state overloads): generate identifier reference
      if (isStateOverload && parameter.Ordinal == 0)
      {
         return SyntaxFactory.IdentifierName(CreateIdentifier(parameter.Name));
      }

      var stateParameterName = isStateOverload ? method.Parameters[0].Name : Constants.Parameters.STATE;

      if (paramType is INamedTypeSymbol namedType)
      {
         // Check for System.Action
         if (namedType.IsSystemAction())
            return BuildActionLambda(namedType, stateParameterName, isStateOverload, reservedNames);

         // Check for System.Func
         if (namedType.IsSystemFunc())
            return BuildFuncLambda(namedType, stateParameterName, isStateOverload, reservedNames);

         // Check for Thinktecture.Argument<T>
         if (namedType.IsThinktectureArgument())
            return SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
      }

      // Non-delegate parameter (Map's TResult values, MapPartially's @default)
      // For Map: default
      // For MapPartially @default: also default (it's required, of type TResult)
      return SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
   }

   private static ExpressionSyntax BuildActionLambda(INamedTypeSymbol actionType, string stateParameterName, bool isStateOverload, HashSet<string> reservedNames)
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
                                SyntaxFactory.Parameter(CreateIdentifier(GetSingleLambdaParameterName(stateParameterName, isStateOverload, reservedNames))),
                                SyntaxFactory.Block())
                             .WithModifiers(staticModifier);
      }

      // Action<T1, T2, ...> → static (x1, x2, ...) => { }
      return SyntaxFactory.ParenthesizedLambdaExpression(
                             SyntaxFactory.ParameterList(BuildMultipleParameters(actionType.TypeArguments.Length, stateParameterName, reservedNames)),
                             SyntaxFactory.Block())
                          .WithModifiers(staticModifier);
   }

   private static ExpressionSyntax BuildFuncLambda(INamedTypeSymbol funcType, string stateParameterName, bool isStateOverload, HashSet<string> reservedNames)
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
                                SyntaxFactory.Parameter(CreateIdentifier(GetSingleLambdaParameterName(stateParameterName, isStateOverload, reservedNames))),
                                throwExpression)
                             .WithModifiers(staticModifier);
      }

      // Func<T1, T2, ..., TResult> → static (x1, x2, ...) => throw new System.NotImplementedException()
      // TypeArguments.Length - 1 because the last type argument is TResult
      return SyntaxFactory.ParenthesizedLambdaExpression(
                             SyntaxFactory.ParameterList(BuildMultipleParameters(funcType.TypeArguments.Length - 1, stateParameterName, reservedNames)),
                             throwExpression)
                          .WithModifiers(staticModifier);
   }

   private static string GetSingleLambdaParameterName(string stateParameterName, bool isStateOverload, HashSet<string> reservedNames)
   {
      // In a state overload every delegate receives the state first, so the only parameter of a
      // single-parameter delegate IS the state (for example a Smart Enum item without a value).
      return isStateOverload
                ? GetStateParameterName(stateParameterName, reservedNames)
                : GetValueParameterName(stateParameterName, reservedNames);
   }

   private static SeparatedSyntaxList<ParameterSyntax> BuildMultipleParameters(int count, string stateParameterName, HashSet<string> reservedNames)
   {
      var nodesAndTokens = new SyntaxNodeOrToken[count * 2 - 1];

      // The state lambda parameter (the first one) must be resolved before the value parameter, so
      // the value parameter can avoid colliding with the possibly renamed state parameter (CS0100).
      var stateName = GetStateParameterName(stateParameterName, reservedNames);
      var valueParameterName = GetValueParameterName(stateName, reservedNames);

      for (var i = 0; i < count; i++)
      {
         if (i > 0)
         {
            nodesAndTokens[i * 2 - 1] = SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                     .WithTrailingTrivia(SyntaxFactory.Space);
         }

         var name = i == 0 ? stateName : valueParameterName;

         // Use CreateIdentifier so a state parameter name that is a C# keyword (e.g. "default") is
         // emitted with the "@" prefix and the generated lambda parses.
         nodesAndTokens[i * 2] = SyntaxFactory.Parameter(CreateIdentifier(name));
      }

      return SyntaxFactory.SeparatedList<ParameterSyntax>(nodesAndTokens);
   }

   private static string GetStateParameterName(string stateParameterName, HashSet<string> reservedNames)
   {
      // The state lambda parameter must not shadow a local or parameter that is already in scope at
      // the invocation (CS0136). The state overload's state argument passes such an enclosing symbol
      // by name, so the configured state parameter name is frequently already reserved. Keep the
      // configured name when it is free; otherwise append a number until the name is free. The lambda
      // body is empty, so the parameter is only a placeholder and can be renamed without changing
      // behavior.
      if (!reservedNames.Contains(stateParameterName))
         return stateParameterName;

      for (var i = 1; ; i++)
      {
         var candidate = stateParameterName + i;

         if (!reservedNames.Contains(candidate))
            return candidate;
      }
   }

   private static string GetValueParameterName(string stateParameterName, HashSet<string> reservedNames)
   {
      // The value lambda parameter must not collide with the state lambda parameter (CS0100, two
      // parameters of the same lambda) and must not shadow a local or parameter that is already in
      // scope at the invocation (CS0136). Pick the first candidate that avoids both; if every
      // candidate is taken, fall back to a numbered name that is guaranteed to be free.
      foreach (var candidate in _valueParameterNameCandidates)
      {
         if (candidate != stateParameterName && !reservedNames.Contains(candidate))
            return candidate;
      }

      for (var i = 1; ; i++)
      {
         var candidate = "value" + i;

         if (candidate != stateParameterName && !reservedNames.Contains(candidate))
            return candidate;
      }
   }

   private static HashSet<string> GetReservedLambdaParameterNames(SemanticModel? semanticModel, InvocationExpressionSyntax invocation)
   {
      // Collect every identifier visible at the invocation so a generated lambda parameter does not
      // shadow an enclosing local or parameter (CS0136). LookupSymbols also returns fields, methods
      // and types; treating those as reserved as well is harmless, because it only makes the picker
      // skip to the next candidate name.
      var reservedNames = new HashSet<string>(StringComparer.Ordinal);

      if (semanticModel is null)
         return reservedNames;

      var symbols = semanticModel.LookupSymbols(invocation.SpanStart);

      if (symbols.IsDefaultOrEmpty)
         return reservedNames;

      foreach (var symbol in symbols)
      {
         reservedNames.Add(symbol.Name);
      }

      return reservedNames;
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
