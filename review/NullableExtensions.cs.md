Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/NullableExtensions.cs

Warnings
1) Overly restrictive generic constraint limits reuse
   - The method &#39;EqualsTo<T>&#39; requires &#39;T : struct, IEquatable<T>&#39;. This prevents use with structs that do not implement &#39;IEquatable<T>&#39;, even though correct equality semantics can still be achieved using &#39;EqualityComparer<T>.Default&#39; together with the existing &#39;HasValue&#39; checks.
   - Suggested fix:
     - Remove the &#39;IEquatable<T>&#39; constraint and implement the value comparison as:
       - if (obj.HasValue != other.HasValue) return false;
       - if (!obj.HasValue) return true;
       - return EqualityComparer<T>.Default.Equals(obj.GetValueOrDefault(), other.GetValueOrDefault());
     - This preserves optimal performance for types that implement &#39;IEquatable<T>&#39; (&#39;EqualityComparer<T>.Default&#39; will dispatch to it) while still working for other structs. Note: For structs without &#39;IEquatable<T>&#39; this may box, which is usually acceptable for a generic helper. If avoiding any boxing is a hard requirement, keep the constraint but document the limitation.

2) Fragile reliance on implicit usings for &#39;IEquatable<T>&#39;
   - The file lacks &#39;using System;&#39; but references &#39;IEquatable<T>&#39;. If implicit usings are disabled, the file will fail to compile.
   - Suggested fix: Add &#39;using System;&#39; at the top (or fully-qualify to &#39;System.IEquatable<T>&#39;).

3) Non-idiomatic method name
   - &#39;EqualsTo&#39; is unconventional and easy to confuse with &#39;object.Equals&#39;. Consider &#39;NullableEquals&#39; or &#39;ValueEquals&#39; to better convey intent. This is cosmetic and does not block functionality.
