using System;
using CsvHelper;

namespace Thinktecture.SmartEnums;

[SmartEnum<string>(KeyPropertyName = "Name")]
public sealed partial class SalesCsvImporterType
{
   public static readonly SalesCsvImporterType Daily = new(name: "Daily", articleIdIndex: 0, volumeIndex: 2, GetDateTimeForDaily);
   public static readonly SalesCsvImporterType Monthly = new(name: "Monthly", articleIdIndex: 2, volumeIndex: 0, GetDateTimeForMonthly);

   public int ArticleIdIndex { get; }
   public int VolumeIndex { get; }

   private readonly Func<CsvReader, DateTime> _getDateTime;
   public DateTime GetDateTime(CsvReader csvReader) => _getDateTime(csvReader);

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
