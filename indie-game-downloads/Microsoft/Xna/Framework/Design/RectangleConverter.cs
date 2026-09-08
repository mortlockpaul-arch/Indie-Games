using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class RectangleConverter : MathTypeConverter
{
	public RectangleConverter()
	{
		Type typeFromHandle = typeof(Rectangle);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[4]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("X")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Y")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Width")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Height"))
		});
		supportStringConvert = false;
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Rectangle && destinationType == typeof(InstanceDescriptor))
		{
			Rectangle rectangle = (Rectangle)value;
			return new InstanceDescriptor(typeof(Rectangle).GetConstructor(new Type[4]
			{
				typeof(int),
				typeof(int),
				typeof(int),
				typeof(int)
			}), new int[4] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new Rectangle((int)propertyValues["X"], (int)propertyValues["Y"], (int)propertyValues["Width"], (int)propertyValues["Height"]);
	}
}
