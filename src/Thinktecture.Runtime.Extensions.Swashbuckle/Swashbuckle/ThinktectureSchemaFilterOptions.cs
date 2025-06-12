namespace Thinktecture.Swashbuckle;

/// <summary>
/// Options for configuring the Thinktecture schema filter.
/// </summary>
public class ThinktectureSchemaFilterOptions
{
   /// <summary>
   /// Gets or sets the schema filter for Smart Enums.
   /// </summary>
   public SmartEnumSchemaFilter SmartEnumSchemaFilter { get; set; } = SmartEnumSchemaFilter.FromDependencyInjection;

   /// <summary>
   /// Gets or sets the schema extension for Smart Enums.
   /// </summary>
   public SmartEnumSchemaExtension SmartEnumSchemaExtension { get; set; } = SmartEnumSchemaExtension.FromDependencyInjection;

   /// <summary>
   /// Gets or sets the required member evaluator.
   /// </summary>
   public RequiredMemberEvaluator RequiredMemberEvaluator { get; set; } = RequiredMemberEvaluator.FromDependencyInjection;

   /// <summary>
   /// Clears 'allOf' from the schema if there is any.
   /// </summary>
   public bool ClearAllOf { get; set; }
}
