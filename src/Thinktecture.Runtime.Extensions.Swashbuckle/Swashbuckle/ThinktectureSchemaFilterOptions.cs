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
   /// Clears 'allOf' from the schema of Smart Enums and keyed Value Objects, which is usually used for inheritance.
   ///
   /// Default is <c>false</c>.
   /// </summary>
   public bool ClearAllOfOnKeyedTypes { get; set; }

   /// <summary>
   /// Gets or sets a value indicating whether to extend the schema ID selector
   /// to produce better schema ids for some internal types.
   ///
   /// Default is <c>true</c>.
   /// </summary>
   public bool ExtendSchemaIdSelector { get; set; } = true;

   /// <summary>
   /// Gets or sets a value indicating whether extra schemas should be created for parameters
   /// if the parameter type differs from the type for model binding.
   ///
   /// Default is <c>true</c>.
   /// </summary>
   public bool CreateExtraSchemasForParameters { get; set; } = true;
}
