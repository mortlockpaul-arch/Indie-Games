using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class Vector4Converter : MathTypeConverter
{
	public Vector4Converter()
	{
		Type typeFromHandle = typeof(Vector4);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[4]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("X")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Y")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Z")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("W"))
		});
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string text)
		{
			StringListEnumerator<float> stringListEnumerator = new StringListEnumerator<float>(culture, text);
			return new Vector4(stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Vector4)
		{
			if (destinationType == typeof(string))
			{
				Vector4 vector = (Vector4)value;
				return MathTypeConverter.ConvertToString<float>(culture, vector.X, vector.Y, vector.Z, vector.W);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Vector4 vector = (Vector4)value;
				return new InstanceDescriptor(typeof(Vector4).GetConstructor(new Type[4]
				{
					typeof(float),
					typeof(float),
					typeof(float),
					typeof(float)
				}), new float[4] { vector.X, vector.Y, vector.Z, vector.W });
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
		return new Vector4((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"], (float)propertyValues["W"]);
	}
}
