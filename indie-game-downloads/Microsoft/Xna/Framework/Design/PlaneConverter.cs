using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class PlaneConverter : MathTypeConverter
{
	public PlaneConverter()
	{
		Type typeFromHandle = typeof(Plane);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[2]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("Normal")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("D"))
		});
		supportStringConvert = false;
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Plane && destinationType == typeof(InstanceDescriptor))
		{
			Plane plane = (Plane)value;
			return new InstanceDescriptor(typeof(Plane).GetConstructor(new Type[2]
			{
				typeof(Vector3),
				typeof(float)
			}), new object[2] { plane.Normal, plane.D });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new Plane((Vector3)propertyValues["Normal"], (float)propertyValues["D"]);
	}
}
