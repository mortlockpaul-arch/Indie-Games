using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class Vector3Converter : MathTypeConverter
{
	public Vector3Converter()
	{
		Type typeFromHandle = typeof(Vector3);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[3]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("X")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Y")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Z"))
		});
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string text)
		{
			StringListEnumerator<float> stringListEnumerator = new StringListEnumerator<float>(culture, text);
			return new Vector3(stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Vector3)
		{
			if (destinationType == typeof(string))
			{
				Vector3 vector = (Vector3)value;
				return MathTypeConverter.ConvertToString<float>(culture, vector.X, vector.Y, vector.Z);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Vector3 vector = (Vector3)value;
				return new InstanceDescriptor(typeof(Vector3).GetConstructor(new Type[3]
				{
					typeof(float),
					typeof(float),
					typeof(float)
				}), new float[3] { vector.X, vector.Y, vector.Z });
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
		return new Vector3((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
	}
}
