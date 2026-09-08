using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class Vector2Converter : MathTypeConverter
{
	public Vector2Converter()
	{
		Type typeFromHandle = typeof(Vector2);
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
			StringListEnumerator<float> stringListEnumerator = new StringListEnumerator<float>(culture, text);
			return new Vector2(stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Vector2)
		{
			if (destinationType == typeof(string))
			{
				Vector2 vector = (Vector2)value;
				return MathTypeConverter.ConvertToString<float>(culture, vector.X, vector.Y);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Vector2 vector = (Vector2)value;
				return new InstanceDescriptor(typeof(Vector2).GetConstructor(new Type[2]
				{
					typeof(float),
					typeof(float)
				}), new float[2] { vector.X, vector.Y });
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
		return new Vector2((float)propertyValues["X"], (float)propertyValues["Y"]);
	}
}
