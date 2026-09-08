using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class PointConverter : MathTypeConverter
{
	public PointConverter()
	{
		Type typeFromHandle = typeof(Point);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[2]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("X")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Y"))
		});
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string text)
		{
			StringListEnumerator<int> stringListEnumerator = new StringListEnumerator<int>(culture, text);
			return new Point(stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Point)
		{
			if (destinationType == typeof(string))
			{
				Point point = (Point)value;
				return MathTypeConverter.ConvertToString<int>(culture, point.X, point.Y);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Point point = (Point)value;
				return new InstanceDescriptor(typeof(Point).GetConstructor(new Type[2]
				{
					typeof(int),
					typeof(int)
				}), new int[2] { point.X, point.Y });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new Point((int)propertyValues["X"], (int)propertyValues["Y"]);
	}
}
