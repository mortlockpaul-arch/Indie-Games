using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class QuaternionConverter : MathTypeConverter
{
	public QuaternionConverter()
	{
		Type typeFromHandle = typeof(Quaternion);
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
			return new Quaternion(stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next(), stringListEnumerator.Next());
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Quaternion)
		{
			if (destinationType == typeof(string))
			{
				Quaternion quaternion = (Quaternion)value;
				return MathTypeConverter.ConvertToString<float>(culture, quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Quaternion quaternion = (Quaternion)value;
				return new InstanceDescriptor(typeof(Quaternion).GetConstructor(new Type[4]
				{
					typeof(float),
					typeof(float),
					typeof(float),
					typeof(float)
				}), new float[4] { quaternion.X, quaternion.Y, quaternion.Z, quaternion.W });
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
		return new Quaternion((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"], (float)propertyValues["W"]);
	}
}
