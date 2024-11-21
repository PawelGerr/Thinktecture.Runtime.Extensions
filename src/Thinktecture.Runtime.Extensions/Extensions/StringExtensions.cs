using System.Runtime.CompilerServices;

namespace Thinktecture;

/// <summary>
/// Extensions methods for <see cref="String"/>.
/// </summary>
public static class StringExtensions
{
   /// <summary>
   /// If the <paramref name="text"/> is <c>null</c>, an empty <see cref="String"/> or contains whitespaces only, then <c>null</c> is returned.
   /// Otherwise, the <paramref name="text"/> is trimmed.
   /// </summary>
   /// <param name="text">Text to trim or nullify.</param>
   /// <returns>
   /// Trimmed <paramref name="text"/>, if the <paramref name="text"/> is not <c>null</c> nor contains whitespaces only;
   /// otherwise <c>null</c>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static string? TrimOrNullify(this string? text)
   {
      return String.IsNullOrWhiteSpace(text) ? null : text.Trim();
   }

   /// <summary>
   /// If the <paramref name="text"/> is <c>null</c>, an empty <see cref="String"/> or contains whitespaces only, then <c>null</c> is returned.
   /// Otherwise, the <paramref name="text"/> is trimmed and shortened according to <paramref name="maxLength"/>.
   /// </summary>
   /// <param name="text">Text to trim or nullify.</param>
   /// <param name="maxLength">The <paramref name="text"/> is shortened if its length exceeds the provided <paramref name="maxLength"/>.</param>
   /// <returns>
   /// Trimmed and shortened (according to <paramref name="maxLength"/>) <paramref name="text"/>, if the <paramref name="text"/> is not <c>null</c> nor contains whitespaces only;
   /// otherwise <c>null</c>.
   /// </returns>
   /// <exception cref="ArgumentException">
   /// <paramref name="maxLength"/> is less or equals to 0.
   /// </exception>
   public static string? TrimOrNullify(this string? text, int maxLength)
   {
      if (maxLength <= 0)
         throw new ArgumentException("The maximum length must be bigger than 0.", nameof(maxLength));

      if (String.IsNullOrWhiteSpace(text))
         return null;

      text = text.Trim();

      if (text.Length > maxLength)
         text = text[..maxLength];

      return text;
   }
}
