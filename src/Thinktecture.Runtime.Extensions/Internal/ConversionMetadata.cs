using System.Linq.Expressions;

namespace Thinktecture.Internal;

/// <summary>
/// Metadata for conversion, serialization, model binding, etc..
/// </summary>
/// <param name="Type">The type to convert.</param>
/// <param name="KeyType">The type to convert to (or from).</param>
/// <param name="ValidationErrorType">The type representing validation errors.</param>
/// <param name="ConvertFromKeyExpressionViaConstructor">Expression to create an instance using constructor.</param>
/// <param name="ConvertFromKeyExpression">Expression to create an instance using factory.</param>
public readonly record struct ConversionMetadata(
   Type Type,
   Type KeyType,
   Type ValidationErrorType,
   LambdaExpression? ConvertFromKeyExpressionViaConstructor,
   LambdaExpression? ConvertFromKeyExpression);
