using System;
using CsvHelper;

namespace Thinktecture.SmartEnums;

[SmartEnum]
public partial class SalesCsvImporterType
{
   public static readonly SalesCsvImporterType Daily = new(articleIdIndex: 0, volumeIndex: 2, GetDateTimeForDaily);
   public static readonly SalesCsvImporterType Monthly = new(articleIdIndex: 2, volumeIndex: 0, GetDateTimeForMonthly);

   public int ArticleIdIndex { get; }
   public int VolumeIndex { get; }

   [UseDelegateFromConstructor]
   public partial DateTime GetDateTime(CsvReader csvReader);

   private static DateTime GetDateTimeForDaily(CsvReader csvReader)
   {
      return DateTime.ParseExact(csvReader[1] ?? throw new Exception("Invalid CSV"),
                                 "yyyyMMdd hh:mm",
                                 null);
   }

   private static DateTime GetDateTimeForMonthly(CsvReader csvReader)
   {
      return csvReader.HeaderRecord?.Length == 3
                ? GetDateTimeForDaily(csvReader)
                : DateTime.ParseExact(csvReader[3] ?? throw new Exception("Invalid CSV"),
                                      "yyyy-MM-dd",
                                      null);
   }
}
