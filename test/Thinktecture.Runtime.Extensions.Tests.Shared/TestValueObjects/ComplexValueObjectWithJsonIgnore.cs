using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class ComplexValueObjectWithJsonIgnore
{
   [JsonIgnore]
   public string? StringProperty_Ignore { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
   public string? StringProperty_Ignore_Always { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
   public string? StringProperty_Ignore_WhenWritingDefault { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   public string? StringProperty_Ignore_WhenWritingNull { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
   public string? StringProperty_Ignore_Never { get; }

   public string? StringProperty { get; }

   [JsonIgnore]
   public int IntProperty_Ignore { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
   public int IntProperty_Ignore_Always { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
   public int IntProperty_Ignore_WhenWritingDefault { get; }

   // WhenWritingNull on structs is not valid
   // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   // public int IntProperty_Ignore_WhenWritingNull { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
   public int IntProperty_Ignore_Never { get; }

   public int IntProperty { get; }

   [JsonIgnore]
   public int? NullableIntProperty_Ignore { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
   public int? NullableIntProperty_Ignore_Always { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
   public int? NullableIntProperty_Ignore_WhenWritingDefault { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
   public int? NullableIntProperty_Ignore_WhenWritingNull { get; }

   [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
   public int? NullableIntProperty_Ignore_Never { get; }

   public int? NullableIntProperty { get; }
}
