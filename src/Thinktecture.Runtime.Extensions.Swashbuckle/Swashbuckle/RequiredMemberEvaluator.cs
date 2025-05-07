using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Swashbuckle.Internal.ComplexValueObjects;

namespace Thinktecture.Swashbuckle;

/// <summary>
/// Determines the implementation of <see cref="IRequiredMemberEvaluator"/>.
/// </summary>
[SmartEnum<string>]
public partial class RequiredMemberEvaluator
{
   /// <summary>
   /// The default required member evaluator.
   ///
   /// The member is considered required:
   /// - if it is a struct value object with <see cref="ValueObjectAttributeBase.AllowDefaultStructs"/> equals to <c>false</c>,
   /// - if it is a non-nullable reference type.
   /// </summary>
   public static readonly RequiredMemberEvaluator Default = new(
      nameof(Default),
      p => ActivatorUtilities.CreateInstance<DefaultRequiredMemberEvaluator>(p));

   /// <summary>
   /// All members are flagged as required.
   /// </summary>
   public static readonly RequiredMemberEvaluator All = new(
      nameof(All),
      p => ActivatorUtilities.CreateInstance<AllRequiredMemberEvaluator>(p));

   /// <summary>
   /// Members are not flagged as required.
   /// </summary>
   public static readonly RequiredMemberEvaluator None = new(
      nameof(None),
      p => ActivatorUtilities.CreateInstance<NoRequiredMemberEvaluator>(p));

   /// <summary>
   /// Evaluator is resolved via dependency injection.
   /// </summary>
   public static readonly RequiredMemberEvaluator FromDependencyInjection = new(
      nameof(FromDependencyInjection),
      p => p.GetRequiredService<IRequiredMemberEvaluator>());

   [UseDelegateFromConstructor]
   internal partial IRequiredMemberEvaluator CreateEvaluator(
      IServiceProvider serviceProvider);
}
