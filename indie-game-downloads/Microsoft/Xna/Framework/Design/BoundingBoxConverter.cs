using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class BoundingBoxConverter : MathTypeConverter
{
	public BoundingBoxConverter()
	{
		Type typeFromHandle = typeof(BoundingBox);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[2]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("Min")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Max"))
		});
		supportStringConvert = false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is BoundingBox && destinationType == typeof(InstanceDescriptor))
		{
			BoundingBox boundingBox = (BoundingBox)value;
			return new InstanceDescriptor(typeof(BoundingBox).GetConstructor(new Type[2]
			{
				typeof(Vector3),
				typeof(Vector3)
			}), new Vector3[2] { boundingBox.Min, boundingBox.Max });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new BoundingBox((Vector3)propertyValues["Min"], (Vector3)propertyValues["Max"]);
	}
}
