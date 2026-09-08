using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class RayConverter : MathTypeConverter
{
	public RayConverter()
	{
		Type typeFromHandle = typeof(Ray);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[2]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("Position")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Direction"))
		});
		supportStringConvert = false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Ray && destinationType == typeof(InstanceDescriptor))
		{
			Ray ray = (Ray)value;
			return new InstanceDescriptor(typeof(Ray).GetConstructor(new Type[2]
			{
				typeof(Vector3),
				typeof(Vector3)
			}), new Vector3[2] { ray.Position, ray.Direction });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new Ray((Vector3)propertyValues["Position"], (Vector3)propertyValues["Direction"]);
	}
}
