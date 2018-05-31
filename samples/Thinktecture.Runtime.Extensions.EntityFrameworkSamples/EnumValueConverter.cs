using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thinktecture.Runtime.Extensions.EntityFrameworkSamples
{
	public class EnumValueConverter<TEnum, TKey> : ValueConverter<TEnum, TKey>
		where TEnum : Enum<TEnum, TKey>
	{
		public EnumValueConverter()
			: base(item => item.Key, key => Enum<TEnum, TKey>.Get(key))
		{
		}
	}
}
