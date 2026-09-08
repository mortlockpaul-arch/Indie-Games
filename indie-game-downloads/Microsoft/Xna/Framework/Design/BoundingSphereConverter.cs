using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Xna.Framework.Design;

public class BoundingSphereConverter : MathTypeConverter
{
	public BoundingSphereConverter()
	{
		Type typeFromHandle = typeof(BoundingSphere);
		propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[2]
		{
			new FieldPropertyDescriptor(typeFromHandle.GetField("Center")),
			new FieldPropertyDescriptor(typeFromHandle.GetField("Radius"))
		});
		supportStringConvert = false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is BoundingSphere && destinationType == typeof(InstanceDescriptor))
		{
			BoundingSphere boundingSphere = (BoundingSphere)value;
			return new InstanceDescriptor(typeof(BoundingSphere).GetConstructor(new Type[2]
			{
				typeof(Vector3),
				typeof(float)
			}), new object[2] { boundingSphere.Center, boundingSphere.Radius });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues", "This method does not accept null for this parameter.");
		}
		return new BoundingSphere((Vector3)propertyValues["Center"], (float)propertyValues["Radius"]);
	}
}
