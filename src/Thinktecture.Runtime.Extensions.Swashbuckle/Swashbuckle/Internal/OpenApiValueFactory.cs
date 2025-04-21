using Microsoft.OpenApi.Any;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
[SmartEnum<Type>]
public partial class OpenApiValueFactory : IOpenApiValueFactory
{
   /// <summary>
   /// Factory for byte values.
   /// </summary>
   public static readonly OpenApiValueFactory Byte = new(typeof(byte), o => new OpenApiString(((byte)o).ToString()));

   /// <summary>
   /// Factory for byte array values.
   /// </summary>
   public static readonly OpenApiValueFactory ByteArray = new(typeof(byte[]), o => new OpenApiString(Convert.ToBase64String((byte[])o)));

   /// <summary>
   /// Factory for boolean values.
   /// </summary>
   public static readonly OpenApiValueFactory Boolean = new(typeof(bool), o => new OpenApiBoolean((bool)o));

   /// <summary>
   /// Factory for 32-bit integer values.
   /// </summary>
   public static readonly OpenApiValueFactory Integer32 = new(typeof(int), o => new OpenApiInteger((int)o));

   /// <summary>
   /// Factory for unsigned 32-bit integer values.
   /// </summary>
   public static readonly OpenApiValueFactory UnsignedInteger32 = new(typeof(uint), o => new OpenApiInteger((int)(uint)o));

   /// <summary>
   /// Factory for 64-bit integer values.
   /// </summary>
   public static readonly OpenApiValueFactory Integer64 = new(typeof(long), o => new OpenApiLong((long)o));

   /// <summary>
   /// Factory for unsigned 64-bit integer values.
   /// </summary>
   public static readonly OpenApiValueFactory UnsignedInteger64 = new(typeof(ulong), o => new OpenApiLong((long)(ulong)o));

   /// <summary>
   /// Factory for single-precision floating-point values.
   /// </summary>
   public static readonly OpenApiValueFactory Float = new(typeof(float), o => new OpenApiFloat((float)o));

   /// <summary>
   /// Factory for double-precision floating-point values.
   /// </summary>
   public static readonly OpenApiValueFactory Double = new(typeof(double), o => new OpenApiDouble((double)o));

   /// <summary>
   /// Factory for decimal values.
   /// </summary>
   public static readonly OpenApiValueFactory Decimal = new(typeof(decimal), o => new OpenApiDouble((double)(decimal)o));

   /// <summary>
   /// Factory for DateTime values.
   /// </summary>
   public static readonly OpenApiValueFactory DateTime = new(typeof(DateTime), o => new OpenApiDateTime((DateTime)o));

   /// <summary>
   /// Factory for DateOnly values.
   /// </summary>
   public static readonly OpenApiValueFactory DateOnly = new(typeof(DateOnly), o => new OpenApiString(((DateOnly)o).ToString("O")));

   /// <summary>
   /// Factory for TimeOnly values.
   /// </summary>
   public static readonly OpenApiValueFactory TimeOnly = new(typeof(TimeOnly), o => new OpenApiString(((TimeOnly)o).ToString("O")));

   /// <summary>
   /// Factory for DateTimeOffset values.
   /// </summary>
   public static readonly OpenApiValueFactory DateTimeOffset = new(typeof(DateTimeOffset), o => new OpenApiDateTime((DateTimeOffset)o));

   /// <summary>
   /// Factory for GUID values.
   /// </summary>
   public static readonly OpenApiValueFactory Guid = new(typeof(Guid), o => new OpenApiString(((Guid)o).ToString("D")));

   /// <summary>
   /// Factory for character values.
   /// </summary>
   public static readonly OpenApiValueFactory Char = new(typeof(char), o => new OpenApiString(((char)o).ToString()));

   /// <summary>
   /// Factory for URI values.
   /// </summary>
   public static readonly OpenApiValueFactory Uri = new(typeof(Uri), o => new OpenApiString(((Uri)o).ToString()));

   /// <summary>
   /// Factory for string values.
   /// </summary>
   public static readonly OpenApiValueFactory String = new(typeof(string), o => new OpenApiString((string)o));

   /// <inheritdoc />
   [UseDelegateFromConstructor]
   public partial IOpenApiAny CreateOpenApiValue(object value);
}
