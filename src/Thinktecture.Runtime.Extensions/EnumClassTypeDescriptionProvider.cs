using System;
using System.ComponentModel;

namespace Thinktecture
{
	/// <summary>
	/// Type descriptor provider for <see cref="EnumClass{TEnum,TKey}"/>.
	/// </summary>
	public class EnumClassTypeDescriptionProvider : TypeDescriptionProvider
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EnumClassTypeDescriptionProvider"/>.
		/// </summary>
		public EnumClassTypeDescriptionProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="EnumClassTypeDescriptionProvider"/>.
		/// </summary>
		/// <param name="parent">Parent provider.</param>
		public EnumClassTypeDescriptionProvider(TypeDescriptionProvider parent)
			: base(parent)
		{
		}

		/// <inheritdoc />
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			var baseDescriptor = base.GetTypeDescriptor(objectType, instance);

			return new EnumClassTypeDescriptor(baseDescriptor, objectType);
		}
	}
}
