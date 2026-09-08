using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class ColorConverter : MathTypeConverter
{
	public ColorConverter()
	{
		Type typeFromHandle = typeof(Color);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[4]
		{
			new PropertyPropertyDescriptor(typeFromHandle.GetProperty("R")),
			new PropertyPropertyDescriptor(typeFromHandle.GetProperty("G")),
			new PropertyPropertyDescriptor(typeFromHandle.GetProperty("B")),
			new PropertyPropertyDescriptor(typeFromHandle.GetProperty("A"))
		});
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string text)
		{
			StringListEnumerator<int> stringListEnumerator = new StringListEnumerator<int>(culture, text);
			return new Color(stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Color)
		{
			if (destinationType == typeof(string))
			{
				Color color = (Color)value;
				return MathTypeConverter.ConvertToString<byte>(culture, color.R, color.G, color.B, color.A);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Color color = (Color)value;
				return new InstanceDescriptor(typeof(Color).GetConstructor(new Type[4]
				{
					typeof(byte),
					typeof(byte),
					typeof(byte),
					typeof(byte)
				}), new byte[4] { color.R, color.G, color.B, color.A });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues");
		}
		return new Color((int)propertyValues["R"], (int)propertyValues["G"], (int)propertyValues["B"], (int)propertyValues["A"]);
	}
}
