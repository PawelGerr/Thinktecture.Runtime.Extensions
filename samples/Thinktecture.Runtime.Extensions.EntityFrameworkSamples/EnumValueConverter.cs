using Microsoft.EntityFrameworkCore.Storage.Converters;

namespace Thinktecture.Runtime.Extensions.EntityFrameworkSamples
{
	public class EnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
		where TEnum : Enum<TEnum, TKey>
	{
		public EnumValueConverter()
			: this(default(ConverterMappingHints))
		{
		}

		public EnumValueConverter(ConverterMappingHints hints)
			: base(item => item.Key, key => Enum<TEnum, TKey>.Get(key), hints)
		{
		}
	}
}