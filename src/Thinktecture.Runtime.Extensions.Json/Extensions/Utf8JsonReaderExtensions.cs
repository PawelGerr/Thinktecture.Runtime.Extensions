using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="Utf8JsonReader"/>
/// </summary>
public static class Utf8JsonReaderExtensions
{
   private static readonly Read<byte> _readByte = GetReadMethod<byte>("GetByteWithQuotes");
   private static readonly Read<sbyte> _readSByte = GetReadMethod<sbyte>("GetSByteWithQuotes");
   private static readonly Read<short> _readShort = GetReadMethod<short>("GetInt16WithQuotes");
   private static readonly Read<ushort> _readUShort = GetReadMethod<ushort>("GetUInt16WithQuotes");
   private static readonly Read<int> _readInt = GetReadMethod<int>("GetInt32WithQuotes");
   private static readonly Read<uint> _readUInt = GetReadMethod<uint>("GetUInt32WithQuotes");
   private static readonly Read<long> _readLong = GetReadMethod<long>("GetInt64WithQuotes");
   private static readonly Read<ulong> _readULong = GetReadMethod<ulong>("GetUInt64WithQuotes");
   private static readonly Read<decimal> _readDecimal = GetReadMethod<decimal>("GetDecimalWithQuotes");
   private static readonly Read<float> _readSingle = GetReadMethod<float>("GetSingleWithQuotes");
   private static readonly Read<float> _readSingleFloatingPointConstant = GetReadMethod<float>("GetSingleFloatingPointConstant");
   private static readonly Read<double> _readDouble = GetReadMethod<double>("GetDoubleWithQuotes");
   private static readonly Read<double> _readDoubleFloatingPointConstant = GetReadMethod<double>("GetDoubleFloatingPointConstant");

   private static readonly Write<long> _writeLong = GetWriteMethod<long>("WriteNumberValueAsString");
   private static readonly Write<ulong> _writeULong = GetWriteMethod<ulong>("WriteNumberValueAsString");
   private static readonly Write<float> _writeSingle = GetWriteMethod<float>("WriteNumberValueAsString");
   private static readonly Write<float> _writeSingleFloatingPointConstant = GetWriteMethod<float>("WriteFloatingPointConstant");
   private static readonly Write<double> _writeDouble = GetWriteMethod<double>("WriteNumberValueAsString");
   private static readonly Write<double> _writeDoubleFloatingPointConstant = GetWriteMethod<double>("WriteFloatingPointConstant");
   private static readonly Write<decimal> _writeDecimal = GetWriteMethod<decimal>("WriteNumberValueAsString");

   private static Read<T> GetReadMethod<T>(string methodName)
   {
      var method = typeof(Utf8JsonReader).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>())
                   ?? throw new Exception($"Method '{methodName}' not found");
      var readerParam = Expression.Parameter(typeof(Utf8JsonReader).MakeByRefType());

      return Expression.Lambda<Read<T>>(Expression.Call(readerParam, method), readerParam).Compile();
   }

   private static Write<T> GetWriteMethod<T>(string methodName)
   {
      var method = typeof(Utf8JsonWriter).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, new[] { typeof(T) })
                   ?? throw new Exception($"Method '{methodName}' not found");
      var writeParam = Expression.Parameter(typeof(Utf8JsonWriter));
      var valueParam = Expression.Parameter(typeof(T));

      return Expression.Lambda<Write<T>>(Expression.Call(writeParam, method, valueParam), writeParam, valueParam).Compile();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static byte GetByteWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readByte(ref reader);

      return reader.GetByte();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static sbyte GetSByteWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readSByte(ref reader);

      return reader.GetSByte();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static short GetShortWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readShort(ref reader);

      return reader.GetInt16();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static ushort GetUShortWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readUShort(ref reader);

      return reader.GetUInt16();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static int GetIntWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readInt(ref reader);

      return reader.GetInt32();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static uint GetUIntWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readUInt(ref reader);

      return reader.GetUInt32();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static long GetLongWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readLong(ref reader);

      return reader.GetInt64();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static ulong GetULongWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readULong(ref reader);

      return reader.GetUInt64();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static decimal GetDecimalWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String && (handling & JsonNumberHandling.AllowReadingFromString) != 0)
         return _readDecimal(ref reader);

      return reader.GetDecimal();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static float GetSingleWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String)
      {
         if ((JsonNumberHandling.AllowReadingFromString & handling) != 0)
         {
            return _readSingle(ref reader);
         }

         if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != 0)
         {
            return _readSingleFloatingPointConstant(ref reader);
         }
      }

      return reader.GetSingle();
   }

   /// <summary>
   /// Reads number according to provided number handling.
   /// </summary>
   /// <param name="reader">Reader.</param>
   /// <param name="handling">Number handling</param>
   /// <returns>Read value.</returns>
   public static double GetDoubleWithNumberHandling(this ref Utf8JsonReader reader, JsonNumberHandling handling)
   {
      if (reader.TokenType == JsonTokenType.String)
      {
         if ((JsonNumberHandling.AllowReadingFromString & handling) != 0)
         {
            return _readDouble(ref reader);
         }

         if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != 0)
         {
            return _readDoubleFloatingPointConstant(ref reader);
         }
      }

      return reader.GetDouble();
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, byte value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, sbyte value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, short value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue((long)value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, ushort value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue((long)value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, int value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue((long)value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, uint value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue((long)value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, long value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeLong(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, ulong value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeULong(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, decimal value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeDecimal(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, float value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeSingle(writer, value);
      }
      else if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != 0)
      {
         _writeSingleFloatingPointConstant(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   /// <summary>
   /// Writes the number according to number handling.
   /// </summary>
   /// <param name="writer">Writer.</param>
   /// <param name="value">Value to write.</param>
   /// <param name="handling">Number handling.</param>
   public static void WriteNumberWithNumberHandling(this Utf8JsonWriter writer, double value, JsonNumberHandling handling)
   {
      if ((JsonNumberHandling.WriteAsString & handling) != 0)
      {
         _writeDouble(writer, value);
      }
      else if ((JsonNumberHandling.AllowNamedFloatingPointLiterals & handling) != 0)
      {
         _writeDoubleFloatingPointConstant(writer, value);
      }
      else
      {
         writer.WriteNumberValue(value);
      }
   }

   private delegate TResult Read<out TResult>(ref Utf8JsonReader reader);

   private delegate void Write<in T>(Utf8JsonWriter writer, T value);
}
