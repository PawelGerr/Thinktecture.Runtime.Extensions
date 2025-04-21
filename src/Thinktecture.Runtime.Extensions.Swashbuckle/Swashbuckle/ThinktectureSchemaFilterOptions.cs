namespace Thinktecture.Swashbuckle;

/// <summary>
/// Options for configuring the Thinktecture schema filter.
/// </summary>
public class ThinktectureSchemaFilterOptions
{
   /// <summary>
   /// Gets or sets the schema filter for Smart Enums.
   ///
   /// Default is <see cref="SmartEnumSchemaFilter.Default"/>.
   /// </summary>
   public SmartEnumSchemaFilter SmartEnumSchemaFilter { get; set; } = SmartEnumSchemaFilter.Default;

   /// <summary>
   /// Gets or sets the schema extension for Smart Enums.
   ///
   /// Default is <see cref="SmartEnumSchemaExtension.None"/>.
   /// </summary>
   public SmartEnumSchemaExtension SmartEnumSchemaExtension { get; set; } = SmartEnumSchemaExtension.None;
}
