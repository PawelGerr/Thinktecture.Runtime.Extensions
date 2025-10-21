using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis.Annotations;

[Generator]
public class AnnotationsSourceGenerator() : ThinktectureSourceGeneratorBase(1), IIncrementalGenerator
{
   private const string _INSTANT_HANDLE_ATTRIBUTE = "InstantHandleAttribute";

   public void Initialize(IncrementalGeneratorInitializationContext context)
   {
      var options = GetGeneratorOptions(context);

      var localAttribute = context.SyntaxProvider
                                  .CreateSyntaxProvider(
                                     (node, _) => node is ClassDeclarationSyntax { Identifier.Text: _INSTANT_HANDLE_ATTRIBUTE },
                                     (ctx, token) => IsInstantHandleAttribute(ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, token)))
                                  .Where(hasLocal => hasLocal)
                                  .Collect();

      var referencedAttribute = context.MetadataReferencesProvider
                                       .Select((reference, _) => HasJetbrainsAnnotations(reference))
                                       .Where(hasAttribute => hasAttribute)
                                       .Collect();

      var pipeline = localAttribute
                     .Combine(referencedAttribute.Combine(options))
                     .SelectMany((tuple, _) =>
                     {
                        var (localAttributes, (referencedAttributes, opts)) = tuple;

                        return !localAttributes.IsDefaultOrEmpty || !referencedAttributes.IsDefaultOrEmpty || !opts.GenerateJetbrainsAnnotations ? ImmutableArray<bool>.Empty : [true];
                     });

      context.RegisterSourceOutput(pipeline, (ctx, _) => AddAnnotations(ctx));
   }

   private static bool IsInstantHandleAttribute(ISymbol? symbol)
   {
      return symbol is
      {
         Name: _INSTANT_HANDLE_ATTRIBUTE, ContainingNamespace:
         {
            Name: "Annotations",
            ContainingNamespace :
            {
               Name: "JetBrains",
               ContainingNamespace.IsGlobalNamespace: true
            }
         }
      };
   }

   private static bool HasJetbrainsAnnotations(MetadataReference reference)
   {
      try
      {
         return reference.GetModules().Any(module => module.Name == "JetBrains.Annotations.dll");
      }
      catch
      {
         return false;
      }
   }

   private void AddAnnotations(SourceProductionContext ctx)
   {
      ctx.AddSource("Thinktecture.Annotations.g.cs",
                    """
                    /* MIT License

                    Copyright (c) 2025 JetBrains http://www.jetbrains.com

                    Permission is hereby granted, free of charge, to any person obtaining a copy
                    of this software and associated documentation files (the "Software"), to deal
                    in the Software without restriction, including without limitation the rights
                    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
                    copies of the Software, and to permit persons to whom the Software is
                    furnished to do so, subject to the following conditions:

                    The above copyright notice and this permission notice shall be included in all
                    copies or substantial portions of the Software.

                    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
                    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
                    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
                    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
                    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
                    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
                    SOFTWARE. */

                    using System;

                    // ReSharper disable once CheckNamespace
                    namespace JetBrains.Annotations;

                    /// <summary>
                    /// Tells the code analysis engine if the parameter is completely handled when the invoked method is on stack.
                    /// If the parameter is of the delegate type - indicates that the delegate can only be invoked during the method
                    /// execution. The delegate can be invoked zero or multiple times, but not stored to some field and invoked later,
                    /// when the containing method is no longer on the execution stack.
                    /// If the parameter is of the enumerable type - indicates that it is enumerated while the method is executed.
                    /// If <see cref="RequireAwait"/> is true - the attribute will only take effect if the method invocation
                    /// is located under the <c>await</c> expression.
                    /// </summary>
                    [AttributeUsage(AttributeTargets.Parameter)]
                    internal sealed class InstantHandleAttribute : Attribute
                    {
                       /// <summary>
                       /// Requires the method invocation to be used under the <c>await</c> expression for this attribute to take effect.
                       /// Can be used for delegate/enumerable parameters of <c>async</c> methods.
                       /// </summary>
                       public bool RequireAwait { get; set; }
                    }
                    """
      );
   }
}
