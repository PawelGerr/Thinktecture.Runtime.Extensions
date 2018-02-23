using System;
using System.ComponentModel;

namespace Thinktecture
{
	/// <summary>
	/// Type descriptor provider for <see cref="Enum{TEnum,TKey}"/>.
	/// </summary>
	public class EnumTypeDescriptionProvider : TypeDescriptionProvider
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EnumTypeDescriptionProvider"/>.
		/// </summary>
		public EnumTypeDescriptionProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="EnumTypeDescriptionProvider"/>.
		/// </summary>
		/// <param name="parent">Parent provider.</param>
		public EnumTypeDescriptionProvider(TypeDescriptionProvider parent)
			: base(parent)
		{
		}

		/// <inheritdoc />
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			var baseDescriptor = base.GetTypeDescriptor(objectType, instance);

			return new EnumTypeDescriptor(baseDescriptor, objectType);
		}
	}
}
